namespace NanoHealthSuite.Data.Models;

public class ValidationLog
{
    public int Id { get; set; }
    public int ProcessExecutionId { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ValidatedAt { get; set; }
    public virtual ProcessExecution ProcessExecution { get; set; }
}