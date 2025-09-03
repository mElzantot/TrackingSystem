namespace NanoHealthSuite.Data.Models;

public class ValidationLog
{
    public int Id { get; set; }
    
    public int ProcessId { get; set; }
    public int StepId { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string RawResponse { get; set; }
    public DateTime ValidatedAt { get; set; }
    public virtual Process Process { get; set; }
    public virtual WorkflowStep WorkflowStep { get; set; }

}