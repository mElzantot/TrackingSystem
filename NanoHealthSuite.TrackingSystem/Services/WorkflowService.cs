using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.Shared;
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

    public async Task<Result<NewWorkflowResponse>> CreateWorkflowAsync(NewWorkflowRequest request, Guid userId)
    {
        var validationResult = ValidateWorkflowDto(request);
        
        if (validationResult.IsFailed)
        {
            return Result.Fail<NewWorkflowResponse>(validationResult.Message);
        }

        var workflow = new Workflow
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
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
                foreach (var validation in stepDto.Validations)
                {
                    var validationEntity = CreateValidation(validation);
                    if (validationEntity.IsFailed)
                    {
                        return Result.Fail<NewWorkflowResponse>(validationEntity.Message);
                    }
                    step.Validations.Add(validationEntity.Value);

                }
            }

            workflow.Steps.Add(step);
            stepMapping[stepDto.Name] = step;
        }

        foreach (var stepDto in request.Steps)
        {
            if (!string.IsNullOrEmpty(stepDto.NextStepName) &&
                stepMapping.TryGetValue(stepDto.NextStepName, out var nextStep))
            {
                var currentStep = stepMapping[stepDto.Name];
                currentStep.NextStep = nextStep;
            }
        }

        await _workflowRepository.AddAsync(workflow);
        _logger.LogInformation("Workflow '{WorkflowName}' created with ID {WorkflowId}", workflow.Name, workflow.Id);
        return Result.Ok(MapToResponseDto(workflow));
    }

    private Result ValidateWorkflowDto(NewWorkflowRequest request)
    {
        var lastStepCount = request.Steps.Count(x => x.NextStepName == null);

        if (lastStepCount > 1)
        {
            return Result.Fail("Duplicate Final step : Two Steps with No Next step Id");
        }
        
        // Validate unique step names
        var duplicateStepNames = request.Steps
            .GroupBy(s => s.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateStepNames.Any())
        {
            return Result.Fail($"Duplicate step names found: {string.Join(", ", duplicateStepNames)}");
        }

        // Validate unique orders
        var duplicateOrders = request.Steps
            .GroupBy(s => s.Order)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateOrders.Any())
        {
            return Result.Fail($"Duplicate step orders found: {string.Join(", ", duplicateOrders)}");
        }

        // Get all valid TempIds
        var validNames = request.Steps.Select(s => s.Name).ToHashSet();

        // Create step order mapping for validation
        var stepOrderMap = request.Steps.ToDictionary(s => s.Name, s => s.Order);

        // Validate NextStepTempId references
        foreach (var step in request.Steps)
        {
            // Check self-reference
            if (step.NextStepName == step.Name)
            {
                return Result.Fail($"Step '{step.Name}' cannot reference itself as next step");
            }

            // Check invalid references (only if NextStepTempId is provided)
            if (!string.IsNullOrEmpty(step.NextStepName) && !validNames.Contains(step.NextStepName))
            {
                return Result.Fail($"Step '{step.Name}' references invalid NextStepTempId: '{step.NextStepName}'");
            }

            // Check order logic - a step can only point to steps with higher order numbers
            if (!string.IsNullOrEmpty(step.NextStepName))
            {
                var currentOrder = step.Order;
                var nextStepOrder = stepOrderMap[step.NextStepName];
                
                if (nextStepOrder <= currentOrder)
                {
                    return Result.Fail($"Step '{step.Name}' (order {currentOrder}) cannot reference step with TempId '{step.NextStepName}' (order {nextStepOrder}). Next step must have a higher order number.");
                }
            }
        }

        return Result.Ok();
    }

    private Result<CustomValidation> CreateValidation(NewCustomValidationDto customValidationRequest)
    {
        var jsonDataValidation = SerializeValidationData(
            customValidationRequest.ValidationType,
            customValidationRequest.Data);

        if (jsonDataValidation.IsFailed)
        {
            return Result.Fail<CustomValidation>(jsonDataValidation.Message);
        }

        return Result.Ok( new CustomValidation
        {
            ValidationType = customValidationRequest.ValidationType,
            Data = jsonDataValidation.Value
        });
    }

    private Result<string> SerializeValidationData(CustomValidationType type, object validationData)
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

    private NewWorkflowResponse MapToResponseDto( Workflow workflow)
    {
        return new NewWorkflowResponse
        {
            Id = workflow.Id,
            Name = workflow.Name,
            Description = workflow.Description,
            CreatedAt = workflow.CreatedAt
        };
    }

    private Result<string> SerializeApiValidation(object data)
    {
        var dataAsString = JsonSerializer.Serialize(data);
        var apiData = JsonSerializer.Deserialize<ApiValidationData>(dataAsString,
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });

        if (string.IsNullOrEmpty(apiData?.Url))
            return Result.Fail<string>("API validation requires a URL");

        return Result.Ok(dataAsString);
    }

    private Result<string> SerializeDatabaseValidation(object data)
    {
        var dataAsString = JsonSerializer.Serialize(data);
        var databaseData = JsonSerializer.Deserialize<DatabaseValidationData>(dataAsString,
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });

        if (string.IsNullOrEmpty(databaseData?.ConnectionStringName))
            return Result.Fail<string>("Database validation requires a ConnectionString");

        return Result.Ok(dataAsString);
    }
}