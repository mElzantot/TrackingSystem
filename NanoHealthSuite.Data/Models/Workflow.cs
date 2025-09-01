namespace NanoHealthSuite.Data.Models;

public class Workflow
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
    public virtual ICollection<Process> Processes { get; set; } = new List<Process>();
}