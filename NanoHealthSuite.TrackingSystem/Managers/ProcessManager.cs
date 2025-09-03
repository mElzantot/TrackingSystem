using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.TrackingSystem.DTOs;
using NanoHealthSuite.TrackingSystem.Services;
using NanoHealthSuite.TrackingSystem.Shared;

namespace NanoHealthSuite.TrackingSystem.Managers;

public class ProcessManager
{
private readonly ILogger<ProcessManager> _logger;
private readonly ValidationService _validationService;
private readonly ProcessService _processService;

public ProcessManager(ILogger<ProcessManager> logger, ValidationService validationService, ProcessService processService)
{
    _logger = logger;
    _validationService = validationService;
    _processService = processService;
}

public async Task<Result<InitiateProcessResponse>> StartProcessAsync(Guid workflowId, Guid userId)
{
    return await _processService.StartProcessAsync(workflowId, userId);
}

public async Task<Result> ExecuteProcessStepAsync(
    Guid userId,
    int processId,
    string stepName,
    UserAction action,
    string comment,
    List<UserInput>? userInputs)
{
    var validationResult = await _validationService.ValidateProcessExecutionAsync(
        userId, 
        processId,
        stepName,
        action,
        userInputs);
    
    if (validationResult.IsFailed)
    {
        return validationResult;
    }
    
    return await _processService.ExecuteProcessStepAsync(
        userId,
        processId,
        stepName,
        action,
        comment,
        userInputs);
}


}