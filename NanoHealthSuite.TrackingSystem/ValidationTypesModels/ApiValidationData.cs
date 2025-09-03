using System.Text.Json.Serialization;

namespace NanoHealthSuite.TrackingSystem.ValidationTypesModels;

public class ApiValidationData
{
    [JsonPropertyName("url")]public string Url { get; set; }
    [JsonPropertyName("method")]public string Method { get; set; } = "POST";
    [JsonPropertyName("headers")]public Dictionary<string, string> Headers { get; set; } = new();
    [JsonPropertyName("payloadTemplate")]public string PayloadTemplate { get; set; }
}
