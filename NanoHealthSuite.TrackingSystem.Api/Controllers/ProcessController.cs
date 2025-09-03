using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.TrackingSystem.Managers;
using NanoHealthSuite.TrackingSystem.Processors;
using NanoHealthSuite.TrackingSystem.Services;

namespace NanoHealthSuite.TrackingSystem.Api.Controllers;

[ApiController]
[Route("api/v1/processes")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProcessController : ControllerBase
{
    private readonly ProcessService _processService;
    private readonly ProcessManager _processManager;
    private readonly ITokenServiceProvider _tokenServiceProvider;
    private readonly ILogger<ProcessController> _logger;

    public ProcessController(
        ProcessService processService, 
        ITokenServiceProvider tokenServiceProvider,
        ILogger<ProcessController> logger, 
        ProcessManager processManager)
    {
        _processService = processService;
        _tokenServiceProvider = tokenServiceProvider;
        _logger = logger;
        _processManager = processManager;
    }

    [HttpPost("start/{workflowId}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartProcessAsync([FromRoute] Guid workflowId)
    {
        try
        {
            var userId = _tokenServiceProvider.GetUserId(User);
            _logger.LogInformation($"Starting new process for workflow {workflowId} by user {userId}");

            var startProcessResult = await _processService.StartProcessAsync(workflowId, userId);

            if (startProcessResult.IsFailed)
            {
                return BadRequest(new { error = startProcessResult.Message });
            }

            _logger.LogInformation("Process started successfully with ID: {ProcessId}", startProcessResult.Value.Id);
            return Ok(startProcessResult.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting process");
            return StatusCode(500, new { error = "An error occurred while starting the process" });
        }
    }

    [HttpPost("execute")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExecuteStepAsync(ExecuteProcessStepRequest request)
    {
        try
        {
            var userId = _tokenServiceProvider.GetUserId(User);

            var executeProcessStepResult = await _processManager.ExecuteProcessStepAsync(
                userId,
                request.ProcessId,
                request.StepName,
                request.Action,
                request.Comment,
                request.UserInputs);

            return executeProcessStepResult.IsFailed
                ? BadRequest(new { error = executeProcessStepResult.Message })
                : Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting process");
            return StatusCode(500, new { error = "An error occurred while starting the process" });
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProcessesAsync(
        [FromQuery] Guid? workflowId,
        [FromQuery] ProcessStatus? status,
        [FromQuery] Guid? userRoleId)
    {
        try
        {
            var result = await _processService.GetProcessesAsync(workflowId, status, userRoleId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Getting Processes");
            return StatusCode(500, new { error = "An error occurred while Getting the process" });
        }
    }

}