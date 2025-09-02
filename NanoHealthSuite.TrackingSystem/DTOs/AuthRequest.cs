using System.ComponentModel.DataAnnotations;
using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.TrackingSystem;

public class AuthRequest
{
    [Required(AllowEmptyStrings = false)]
    public string UserName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Email { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; }
    public Guid RoleId { get; set; }

}