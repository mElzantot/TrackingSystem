using System.Xml;

namespace NanoHealthSuite.TrackingSystem;

public class NewCustomValidationResponse
{
    public int Id { get; set; }
    public ValidationType ValidationType { get; set; }
    public object Data { get; set; }

}