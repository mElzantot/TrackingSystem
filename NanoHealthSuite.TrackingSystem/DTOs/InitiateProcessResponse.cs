using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.TrackingSystem;

public class InitiateProcessResponse
{
    public int Id { get; set; }  
    public Guid InitiatorId { get; set; }
    public Guid WorkflowId { get; set; }
    public ProcessStatus Status { get; set; }
    public string CurrentStepName { get; set; }   
    public string WorkflowName { get; set; }

}