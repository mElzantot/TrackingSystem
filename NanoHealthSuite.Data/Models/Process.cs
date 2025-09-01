using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.Data.Models;

public class Process
{
    public int Id { get; set; }
    public Guid WorkflowId { get; set; }
    public Guid InitiatorId { get; set; }
    public int CurrentStepId { get; set; }
    public ProcessStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public virtual Workflow Workflow { get; set; }
    public virtual User Initiator { get; set; }
    public virtual WorkflowStep CurrentStep { get; set; }
    public virtual ICollection<ProcessExecution> Executions { get; set; } = new List<ProcessExecution>();
    public virtual ICollection<ValidationLog> ValidationLogs { get; set; } = new List<ValidationLog>();

}