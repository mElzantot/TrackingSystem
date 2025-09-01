namespace NanoHealthSuite.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public string? PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpirationDate { get; set; }
    public DateTime CreationDate { get; set; }
    public virtual UserRole Role { get; set; }
    public virtual List<Process> InitiatedProcesses { get; set; }
    public virtual List<ProcessExecution> ProcessExecutions { get; set; }


}