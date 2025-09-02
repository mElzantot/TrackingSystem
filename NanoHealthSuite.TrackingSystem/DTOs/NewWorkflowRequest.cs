using System.ComponentModel.DataAnnotations;

namespace NanoHealthSuite.TrackingSystem;

public class NewWorkflowRequest
{
    [Required(ErrorMessage = "Workflow name is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "At least one step is required")]
    [MinLength(1, ErrorMessage = "Workflow must have at least one step")]
    public List<NewWorkflowStepDto> Steps { get; set; } = new List<NewWorkflowStepDto>();

}