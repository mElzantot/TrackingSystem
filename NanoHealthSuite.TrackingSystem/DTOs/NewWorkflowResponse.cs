namespace NanoHealthSuite.TrackingSystem;

public class NewWorkflowResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<NewWorkflowStepDto> Steps { get; set; } = [];

}