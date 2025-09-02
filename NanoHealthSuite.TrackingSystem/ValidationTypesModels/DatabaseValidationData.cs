namespace NanoHealthSuite.TrackingSystem.ValidationTypesModels;

public class DatabaseValidationData
{
    public string ConnectionStringName { get; set; }
    public string Query { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}
