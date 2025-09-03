using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.TrackingSystem;

public class NewCustomValidationDto
{
    [Required(ErrorMessage = "Validation type is required")]
    public CustomValidationType ValidationType { get; set; }

    [Required(ErrorMessage = "Validation data is required")]
    public object Data { get; set; }

}