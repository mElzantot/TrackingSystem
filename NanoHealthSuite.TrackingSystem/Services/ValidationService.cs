using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.DTOs;
using NanoHealthSuite.TrackingSystem.Shared;
using NanoHealthSuite.Validation;

namespace NanoHealthSuite.TrackingSystem.Services;

public class ValidationService
{
    private readonly Func<CustomValidationType, IProcessStepValidationStrategy> _validationStrategy;
    private readonly IValidationLogRepository _validationLogRepository;
    private readonly IProcessRepository _processRepository;
    private readonly IUserRepository _userRepository;
    private readonly ProcessHelper _processHelper;
    
    public ValidationService(
        Func<CustomValidationType, IProcessStepValidationStrategy> validationStrategies,
        IValidationLogRepository validationLogRepository, 
        IProcessRepository processRepository,
        IUserRepository userRepository,
        ProcessHelper processHelper)
    {
        _validationStrategy = validationStrategies;
        _validationLogRepository = validationLogRepository;
        _processRepository = processRepository;
        _userRepository = userRepository;
        _processHelper = processHelper;
    }


    public async Task<Result> ValidateProcessExecutionAsync(
        Guid userId,
        int processId,
        string stepName,
        UserAction action,
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

        if (process == null)
        {
            return Result.Fail($"Process with Id '{processId}' doesn't exist");
        }

        if (process.CurrentStep == null || !string.Equals(process.CurrentStep.Name, stepName))
        {
            return Result.Fail($"Process is not in '{stepName}' Step , Action can not be Taken");
        }

        var user = await _userRepository.GetFirstAsync(
            x => x,
            x => x.Id == userId);

        if (user == null || user.RoleId != process.CurrentStep.AssignedRole)
        {
            return Result.Fail("User Can't take an action on this Step , Action can not be Taken");
        }

        if (process.CurrentStep.ActionType == ActionType.Input && (userInputs == null || userInputs.Count == 0))
        {
            return Result.Fail("Input data is required for input steps");
        }

        if (process.CurrentStep.Validations.Any() && _processHelper.IsApprovedAction(action))
        {
            foreach (var validation in process.CurrentStep.Validations)
            {
                var validationResult = await ValidateStepAsync(
                    process.Id,
                    process.CurrentStep.Id,
                    process.CurrentStep.Name,
                    userId,
                    action,
                    validation);

                if (!validationResult.IsSuccess)
                {
                    return Result.Fail($"Validation failed: {validationResult.Message}");
                }
            }
        }

        return Result.Ok();
    }


    public async Task<Result> ValidateStepAsync(
        int processId, 
        int stepId,
        string stepName,
        Guid userId,
        UserAction action,
        CustomValidation validation)
    {
        
        
        var data = new Dictionary<string, string>()
        {
            { "processId", processId.ToString() },
            { "stepId", stepId.ToString() },
            { "stepName", stepName },
            { "userId", userId.ToString() },
            { "action", action.ToString() },
        };

        var validator = _validationStrategy.Invoke(validation.ValidationType);
        var validationResult =  await validator.ValidateAsync(data, validation);
        if (validationResult.IsSuccess)
        {
            await _validationLogRepository.AddAsync(new ValidationLog
            {
                ProcessId = processId,
                StepId = stepId,
                IsSuccess = validationResult.IsSuccess,
                ErrorMessage  = validationResult.IsFailed ? validationResult.Message : string.Empty,
                ValidatedAt = DateTime.UtcNow,
                RawResponse = validationResult?.Value ??  string.Empty
            });
        }

        return validationResult.IsSuccess ? Result.Ok() : Result.Fail(validationResult.Message);
    }
}