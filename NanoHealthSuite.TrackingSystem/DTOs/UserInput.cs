using System.ComponentModel.DataAnnotations;

namespace NanoHealthSuite.TrackingSystem.DTOs;

public class UserInput
{
    [Required(ErrorMessage = "Field name is required")]
    public string FieldName { get; set; }
    
    [Required(ErrorMessage = "Field value is required")]
    public string FieldValue { get; set; }
    
    [Required(ErrorMessage = "Field type is required")]
    public string FieldType { get; set; }  // "text", "number", "date", "boolean", "file", etc.
}