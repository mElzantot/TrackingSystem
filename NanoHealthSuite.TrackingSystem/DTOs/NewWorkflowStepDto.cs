using System.ComponentModel.DataAnnotations;
using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.TrackingSystem;

public class NewWorkflowStepDto
{
    [Required(ErrorMessage = "Step name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Step name must be between 1 and 200 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "AssignedTo is required")]
    public Guid AssignedRole { get; set; }

    [Required(ErrorMessage = "Action type is required")]
    public ActionType ActionType { get; set; }
    public string? NextStepName { get; set; }
    public List<NewCustomValidationDto>? Validations { get; set; } = null;

}