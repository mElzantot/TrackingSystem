using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.Data.Models;

public class WorkflowStep
{
    public int Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string Name { get; set; }
    public Guid UserRoleId { get; set; }
    public ActionType ActionType { get; set; } 
    public int? NextStepId { get; set; } 
    public virtual Workflow Workflow { get; set; }
    public virtual WorkflowStep? NextStep { get; set; }
    public virtual WorkflowStep? PreviousStepOf { get; set; }
    public virtual ICollection<CustomValidation> Validations { get; set; } 
    public virtual ICollection<Process> Processes { get; set; }
    public virtual ICollection<ProcessExecution> ProcessExecutions { get; set; }
    public virtual UserRole UserRole { get; set; }



}