using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanoHealthSuite.TrackingSystem.Processors;
using NanoHealthSuite.TrackingSystem.Services;

namespace NanoHealthSuite.TrackingSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/v1/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowService _workflowService;
    private readonly ITokenServiceProvider _tokenServiceProvider;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(
        WorkflowService workflowService, 
        ITokenServiceProvider tokenServiceProvider,
        ILogger<WorkflowController> logger)
    {
        _workflowService = workflowService;
        _tokenServiceProvider = tokenServiceProvider;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(NewWorkflowResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateWorkflow([FromBody] NewWorkflowRequest request)
    {
        try
        {
            var userId = _tokenServiceProvider.GetUserId(User);

            _logger.LogInformation("Creating new workflow: {WorkflowName}", request.Name);

            var result = await _workflowService.CreateWorkflowAsync(request, userId);

            if (result.IsFailed)
            {
                return BadRequest(new { error = result.Message });
            }

            _logger.LogInformation("Workflow created successfully with ID: {WorkflowId}", result.Value.Id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workflow");
            return StatusCode(500, new { error = "An error occurred while creating the workflow" });
        }
    }

}