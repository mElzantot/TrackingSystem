namespace NanoHealthSuite.Data.Models;

public class UserRole
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public virtual List<User> Users { get; set; }
}