namespace NanoHealthSuite.TrackingSystem.ValidationTypesModels;

public class ApiValidationData
{
    public string Url { get; set; }
    public string Method { get; set; } = "POST";
    public Dictionary<string, string> Headers { get; set; } = new();
    public string PayloadTemplate { get; set; }
}
