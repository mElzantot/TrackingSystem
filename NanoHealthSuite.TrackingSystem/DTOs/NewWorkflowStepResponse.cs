using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.TrackingSystem;

public class NewWorkflowStepResponse
{
    public int Id { get; set; }
        
    public string Name { get; set; }
        
    public Guid AssignedRole { get; set; }
        
    public ActionType ActionType { get; set; }
        
    public string? NextStep { get; set; }

    public List<NewCustomValidationResponse>? Validations { get; set; } = null;

}