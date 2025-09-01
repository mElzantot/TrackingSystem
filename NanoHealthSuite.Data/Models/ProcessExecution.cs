namespace NanoHealthSuite.Data.Models;

public class ProcessExecution
{
    public int Id { get; set; }
    public int ProcessId { get; set; }
    public int StepId { get; set; }
    public Guid UserId { get; set; } 
    public string Action { get; set; }  //TODO : Create Action , and ActionType tables to unify Names of taken actions along the whole system 
    public DateTime ExecutedAt { get; set; }
    public string? Comments { get; set; } 
    public virtual User User { get; set; }
    public virtual Process Process { get; set; }
    public virtual WorkflowStep WorkflowStep { get; set; }
    public virtual List<ValidationLog> ValidationLogs { get; set; }

}