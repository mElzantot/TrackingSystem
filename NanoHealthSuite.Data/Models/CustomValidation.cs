using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.Data.Models;

public class CustomValidation
{
    public int Id { get; set; }
    public int StepId { get; set; }
    public ValidationType ValidationType { get; set; }
    public string Data { get; set; } 
    public virtual WorkflowStep Step { get; set; }
}