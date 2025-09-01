using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.Data.Models;

public class WorkflowStep
{
    public int Id { get; set; }
    public int WorkflowId { get; set; }
    public string StepName { get; set; }
    public string AssignedTo { get; set; }
    public ActionType ActionType { get; set; } 
    public int? NextStepId { get; set; } 
    public virtual Workflow Workflow { get; set; }
    public virtual WorkflowStep? NextStep { get; set; }
    public virtual WorkflowStep? PreviousStepOf { get; set; }
    public virtual ICollection<CustomValidation> Validations { get; set; } 
    public virtual ICollection<Process> Processes { get; set; }
    public virtual ICollection<ProcessExecution> ProcessExecutions { get; set; }


}