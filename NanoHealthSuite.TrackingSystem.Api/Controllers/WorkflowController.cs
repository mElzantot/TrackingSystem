using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NanoHealthSuite.TrackingSystem.Services;

namespace NanoHealthSuite.TrackingSystem.Api.Controllers;

[ApiController]
[Route("api/v1/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowService _workflowService;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(WorkflowService workflowService, ILogger<WorkflowController> logger)
    {
        _workflowService = workflowService;
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
            _logger.LogInformation("Creating new workflow: {WorkflowName}", request.Name);

            var result = await _workflowService.CreateWorkflowAsync(request);

            _logger.LogInformation("Workflow created successfully with ID: {WorkflowId}", result.Id);

            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation error creating workflow: {Error}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workflow");
            return StatusCode(500, new { error = "An error occurred while creating the workflow" });
        }
    }

}