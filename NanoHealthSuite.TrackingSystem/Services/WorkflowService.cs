using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.ValidationTypesModels;

namespace NanoHealthSuite.TrackingSystem.Services;

public class WorkflowService
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(IWorkflowRepository workflowRepository, ILogger<WorkflowService> logger)
    {
        _workflowRepository = workflowRepository;
        _logger = logger;
    }

    public async Task<NewWorkflowResponse> CreateWorkflowAsync(NewWorkflowRequest request)
    {
        ValidateWorkflowDto(request);

        var workflow = new Workflow
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            Steps = new List<WorkflowStep>()
        };

        var stepMapping = new Dictionary<string, WorkflowStep>();

        // Create steps in order
        foreach (var stepDto in request.Steps.OrderBy(s => s.Order))
        {
            var step = new WorkflowStep
            {
                Name = stepDto.Name,
                UserRoleId = stepDto.AssignedRole,
                ActionType = stepDto.ActionType,
                Workflow = workflow,
                Validations = new List<CustomValidation>()
            };

            if (stepDto.Validations != null && stepDto.Validations.Any())
            {
                foreach (var validation in stepDto.Validations.Select(CreateValidation))
                {
                    step.Validations.Add(validation);
                }
            }

            workflow.Steps.Add(step);
            stepMapping[stepDto.TempId] = step;
        }

        foreach (var stepDto in request.Steps)
        {
            if (!string.IsNullOrEmpty(stepDto.NextStepTempId) &&
                stepMapping.TryGetValue(stepDto.NextStepTempId, out var nextStep))
            {
                var currentStep = stepMapping[stepDto.TempId];
                currentStep.NextStep = nextStep;
            }
        }

        await _workflowRepository.AddAsync(workflow);
        _logger.LogInformation("Workflow '{WorkflowName}' created with ID {WorkflowId}", workflow.Name, workflow.Id);
        return MapToResponseDto(workflow);
    }

    private void ValidateWorkflowDto(NewWorkflowRequest request)
    {
        // Validate unique TempIds
        var duplicateTempIds = request.Steps
            .GroupBy(s => s.TempId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateTempIds.Any())
        {
            throw new ValidationException($"Duplicate TempIds found: {string.Join(", ", duplicateTempIds)}");
        }

        // Validate unique step names
        var duplicateStepNames = request.Steps
            .GroupBy(s => s.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateStepNames.Any())
        {
            throw new ValidationException($"Duplicate step names found: {string.Join(", ", duplicateStepNames)}");
        }

        // Validate unique orders
        var duplicateOrders = request.Steps
            .GroupBy(s => s.Order)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateOrders.Any())
        {
            throw new ValidationException($"Duplicate step orders found: {string.Join(", ", duplicateOrders)}");
        }

        // Get all valid TempIds
        var validTempIds = request.Steps.Select(s => s.TempId).ToHashSet();

        // Create step order mapping for validation
        var stepOrderMap = request.Steps.ToDictionary(s => s.TempId, s => s.Order);

        // Validate NextStepTempId references
        foreach (var step in request.Steps)
        {
            // Check self-reference
            if (step.NextStepTempId == step.TempId)
            {
                throw new ValidationException($"Step '{step.Name}' cannot reference itself as next step");
            }

            // Check invalid references (only if NextStepTempId is provided)
            if (!string.IsNullOrEmpty(step.NextStepTempId) && !validTempIds.Contains(step.NextStepTempId))
            {
                throw new ValidationException($"Step '{step.Name}' references invalid NextStepTempId: '{step.NextStepTempId}'");
            }

            // Check order logic - a step can only point to steps with higher order numbers
            if (!string.IsNullOrEmpty(step.NextStepTempId))
            {
                var currentOrder = step.Order;
                var nextStepOrder = stepOrderMap[step.NextStepTempId];
                
                if (nextStepOrder <= currentOrder)
                {
                    throw new ValidationException($"Step '{step.Name}' (order {currentOrder}) cannot reference step with TempId '{step.NextStepTempId}' (order {nextStepOrder}). Next step must have a higher order number.");
                }
            }
        }
    }

    private CustomValidation CreateValidation(NewCustomValidationDto customValidationRequest)
    {
        var jsonData = SerializeValidationData(
            customValidationRequest.ValidationType,
            customValidationRequest.Data);

        return new CustomValidation
        {
            ValidationType = customValidationRequest.ValidationType,
            Data = jsonData
        };
    }

    private string SerializeValidationData(CustomValidationType type, object validationData)
    {
        try
        {
            return type switch
            {
                CustomValidationType.API => SerializeApiValidation(validationData),
                CustomValidationType.Database => SerializeDatabaseValidation(validationData),
                _ => throw new ArgumentException($"Unknown validation type: {type}")
            };
        }
        catch (JsonException ex)
        {
            throw new ValidationException($"Invalid validation data format for type {type}: {ex.Message}");
        }
    }

    private NewWorkflowResponse MapToResponseDto(Workflow workflow)
    {
        return new NewWorkflowResponse
        {
            Id = workflow.Id,
            Name = workflow.Name,
            Description = workflow.Description,
            CreatedAt = workflow.CreatedAt
        };
    }

    private string SerializeApiValidation(object data)
    {
        var apiData = JsonSerializer.Deserialize<ApiValidationData>(JsonSerializer.Serialize(data));

        if (string.IsNullOrEmpty(apiData?.Url))
            throw new ValidationException("API validation requires a URL");

        return JsonSerializer.Serialize(apiData);
    }

    private string SerializeDatabaseValidation(object data)
    {
        var databaseData = JsonSerializer.Deserialize<DatabaseValidationData>(JsonSerializer.Serialize(data));

        if (string.IsNullOrEmpty(databaseData?.ConnectionStringName))
            throw new ValidationException("Database validation requires a ConnectionString");

        return JsonSerializer.Serialize(databaseData);
    }
}