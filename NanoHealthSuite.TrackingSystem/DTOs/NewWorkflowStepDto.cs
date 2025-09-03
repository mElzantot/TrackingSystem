using System.ComponentModel.DataAnnotations;
using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.TrackingSystem;

public class NewWorkflowStepDto
{
    [Required(ErrorMessage = "TempId is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "TempId must be between 1 and 50 characters")]
    public string TempId { get; set; }

    [Required(ErrorMessage = "Step name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Step name must be between 1 and 200 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Order is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Order must be greater than 0")]
    public int Order { get; set; }

    [Required(ErrorMessage = "AssignedRole is required")]
    public Guid AssignedRole { get; set; }

    [Required(ErrorMessage = "Action type is required")]
    public ActionType ActionType { get; set; }
    
    public string? NextStepTempId { get; set; }
    public List<NewCustomValidationDto>? Validations { get; set; } = null;
}