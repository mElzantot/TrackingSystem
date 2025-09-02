using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.ValidationTypesModels;
using WorkflowTracking.Infrastructure.Data;

namespace NanoHealthSuite.TrackingSystem.Services;

public class WorkflowService
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(WorkflowDbContext context, ILogger<WorkflowService> logger)
    {
        _context = context;
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

        // Create steps with validations
        var stepMapping = new Dictionary<string, WorkflowStep>();

        foreach (var stepDto in request.Steps)
        {
            var step = new WorkflowStep
            {
                Name = stepDto.Name,
                UserRoleId = stepDto.AssignedRole,
                ActionType = stepDto.ActionType,
                Workflow = workflow,
                Validations = new List<CustomValidation>()
            };

            // Add validations if any
            if (stepDto.Validations != null && stepDto.Validations.Any())
            {
                foreach (var validationDto in stepDto.Validations)
                {
                    var validation = CreateValidation(validationDto);
                    step.Validations.Add(validation);
                }
            }

            workflow.Steps.Add(step);
            stepMapping[step.Name] = step;
        }

        // Link NextStep references
        for (int i = 0; i < request.Steps.Count; i++)
        {
            var stepDto = request.Steps[i];
            var step = workflow.Steps.ElementAt(i);

            if (!string.IsNullOrEmpty(stepDto.NextStepName) &&
                stepMapping.TryGetValue(stepDto.NextStepName, out var nextStep))
            {
                step.NextStep = nextStep;
            }
        }

        // Save to database
        _context.Workflows.Add(workflow);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Workflow '{WorkflowName}' created with ID {WorkflowId}", workflow.Name, workflow.Id);

        // Return response DTO
        return MapToResponseDto(workflow);
    }

    private void ValidateWorkflowDto(NewWorkflowRequest request)
    {
        var duplicateSteps = request.Steps
            .GroupBy(s => s.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateSteps.Any())
        {
            throw new ValidationException($"Duplicate step names found: {string.Join(", ", duplicateSteps)}");
        }

        // Check for circular references
        var stepNames = request.Steps.Select(s => s.Name).ToHashSet();
        foreach (var step in request.Steps)
        {
            if (step.NextStepName == step.Name)
            {
                throw new ValidationException($"Step '{step.Name}' cannot reference itself as next step");
            }
        }
    }

    private CustomValidation CreateValidation(NewCustomValidationDto customValidationRequest)
    {
        // Serialize validation data based on type
        var jsonData =
            SerializeValidationData(customValidationRequest.ValidationType, customValidationRequest.Data);

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