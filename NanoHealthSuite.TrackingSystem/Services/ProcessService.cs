using System.Text.Json;
using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.Shared;
using NanoHealthSuite.TrackingSystem.DTOs;

namespace NanoHealthSuite.TrackingSystem.Services;

public class ProcessService
{
    private readonly IProcessRepository _processRepository;
    private readonly IProcessExecutionRepository _processExecutionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    private readonly ILogger<ProcessService> _logger;

    public ProcessService(
        IProcessRepository processRepository, 
        IWorkflowRepository workflowRepository,
        ILogger<ProcessService> logger,
        IProcessExecutionRepository processExecutionRepository)
    {
        _processRepository = processRepository;
        _workflowRepository = workflowRepository;
        _logger = logger;
        _processExecutionRepository = processExecutionRepository;
    }

    public async Task<Result<InitiateProcessResponse>> StartProcessAsync(Guid workflowId, Guid userId)
    {
        var workflow = await _workflowRepository.GetSingleAsync(
                x => x,
                x => x.Id == workflowId,
                includes: x => x.Steps);

        if (workflow == null)
        {
            return Result.Fail<InitiateProcessResponse>($"Workflow with ID {workflowId} not found");
        }

        if (workflow.Steps.Count == 0)
        {
            return Result.Fail<InitiateProcessResponse>($"Workflow '{workflow.Name}' has no steps defined");
        }

        var firstStep = GetFirstStepId(workflow.Steps);

        if (firstStep == 0)
        {
            return Result.Fail<InitiateProcessResponse>($"Could not determine the first step for workflow '{workflow.Name}'");
        }

        var process = new Process
        {
            WorkflowId = workflow.Id,
            InitiatorId = userId,
            CurrentStepId = firstStep,
            Status = ProcessStatus.Active,
            StartedAt = DateTime.UtcNow,
            Executions = new List<ProcessExecution>()
        };

        await _processRepository.AddAsync(process);

        process.Workflow = workflow;
        process.CurrentStep = workflow.Steps.FirstOrDefault(x => x.Id == firstStep);

        var result = MapToResponseDto(process);
        
        return Result.Ok(result);
    }

    public async Task<Result> ExecuteProcessStepAsync(
        Guid userId,
        int processId,
        string stepName,
        UserAction action,
        string comment,
        List<UserInput>? userInputs)
    {
        var process = await _processRepository.GetSingleAsync(
            x => new
            {
                Id = x.Id,
                CurrentStep = new
                {
                    Id = x.CurrentStep.Id,
                    Name = x.CurrentStep.Name,
                    ActionType = x.CurrentStep.ActionType,
                    AssignedRole = x.CurrentStep.UserRoleId,
                    Validations = x.CurrentStep.Validations
                }
            },
            x => x.Id == processId);

        await LogProcessExecutionAsync(
            process.Id,
            process.CurrentStep.Id,
            userId,
            action,
            comment,
            userInputs);

        if (IsApprovedAction(action))
        {
            await MoveToNextStepAsync(process.Id);
        }
        else
        {
            await EndProcessAsync(process.Id, ProcessStatus.Rejected);
        }

        return Result.Ok();
    }

    public async Task<List<ProcessDto>> GetProcessesAsync(Guid? workflowId, ProcessStatus? status, Guid? userRoleId)
    {
        var processes = await _processRepository.GetAllAsync(
            selector: x => new ProcessDto()
            {
                Id = x.Id,
                WorkflowId = x.WorkflowId,
                WorkflowName = x.Workflow.Name,
                InitiatorId = x.InitiatorId,
                InitiatorName = x.Initiator.Name,
                Status = x.Status,
                StartedAt = x.StartedAt,
                CompletedAt = x.CompletedAt,
                CurrentStepName = x.CurrentStep.Name,
                AssignedToRole = x.CurrentStep.UserRole.Name
            },
            predicate: x =>
                (workflowId == null || x.WorkflowId == workflowId.Value) &&
                (status == null || x.Status == status) &&
                (userRoleId == null || x.CurrentStep.UserRoleId == userRoleId));

        return processes.Count == 0 ? Enumerable.Empty<ProcessDto>().ToList() : processes;
    }

    private int GetFirstStepId(ICollection<WorkflowStep> steps)
    {
        var stepsIds = steps.Select(x => x.Id).ToList();
        var nextStepsIds = steps.Where(x => x.NextStepId != null).Select(x => x.NextStepId!.Value).ToList();
        return stepsIds.Except(nextStepsIds).FirstOrDefault();
    }

    private InitiateProcessResponse MapToResponseDto(Process process)
    {
        return new InitiateProcessResponse
        {
            Id = process.Id,
            WorkflowId = process.WorkflowId,
            WorkflowName = process.Workflow?.Name,
            InitiatorId = process.InitiatorId,
            Status = process.Status,
            CurrentStepName =  process.CurrentStep?.Name,
        };
    }

    private bool IsApprovedAction(UserAction action)
    {
        return action is UserAction.Approve or UserAction.Submit;
    }

    private async Task LogProcessExecutionAsync(
        int processId, 
        int stepId, 
        Guid userId, 
        UserAction action,
        string comment,
        List<UserInput>? userInputs)
    {
        var execution = new ProcessExecution
        {
            ProcessId = processId,
            StepId = stepId,
            UserId = userId,
            Action = action,
            ExecutedAt = DateTime.UtcNow,
            Comments = comment,
            UserInputs = userInputs != null ?  JsonSerializer.Serialize(userInputs) : string.Empty
        };

        await _processExecutionRepository.AddAsync(execution);

        _logger.LogInformation(
            "Process execution logged: ProcessId={ProcessId}, StepId={StepId}, UserId={UserId}, Action={Action}",
            processId, stepId, userId, action);
    }

    private async Task MoveToNextStepAsync(int processId)
    {
        var process = await _processRepository.GetSingleAsync(
            x => x,
            x => x.Id == processId,
            includes: x => x.CurrentStep);

        if (process == null) return;
        var nextStepId = process.CurrentStep.NextStepId;

        if (nextStepId != null)
        {
            await _processRepository.UpdateAsync(
                processId,
                process => process.CurrentStepId = nextStepId.Value);
        }
        else
        {
            await EndProcessAsync(processId, ProcessStatus.Completed);
        }
    }

    private async Task EndProcessAsync(int processId, ProcessStatus status)
    {
        var process = await _processRepository.GetSingleAsync(x => x, x => x.Id == processId);
        if (process == null) return;

        await _processRepository.UpdateAsync(process.Id, process =>
        {
            process.Status = status;
            process.CompletedAt = DateTime.UtcNow;
        });

        _logger.LogInformation("Process {ProcessId} ended with status {Status}", processId, status);
    }
}