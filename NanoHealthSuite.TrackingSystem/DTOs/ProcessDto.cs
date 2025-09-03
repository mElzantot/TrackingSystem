using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.TrackingSystem;

public class ProcessDto
{
    public int Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string WorkflowName { get; set; }
    public Guid InitiatorId { get; set; }
    public string InitiatorName { get; set; }
    public ProcessStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CurrentStepName { get; set; }
    public string? AssignedToRole { get; set; }
}