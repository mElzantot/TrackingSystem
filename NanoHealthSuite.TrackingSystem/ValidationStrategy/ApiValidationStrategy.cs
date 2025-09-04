using System.Text.Json;
using System.Text.RegularExpressions;
using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.Shared;
using NanoHealthSuite.TrackingSystem.ValidationTypesModels;

namespace NanoHealthSuite.Validation;

public class ApiValidationStrategy : HttpApiClient, IProcessStepValidationStrategy 
{
    private readonly HttpClient _httpClient;
    
    public ApiValidationStrategy(HttpClient _httpClient) :base(_httpClient)
    {
    }

    public async Task<Result<string>> ValidateAsync(
        Dictionary<string, string> data,
        CustomValidation validation)
    {
        var apiData = JsonSerializer.Deserialize<ApiValidationData>(validation.Data);

        if (string.IsNullOrEmpty(apiData?.Url))
        {
            return Result.Fail<string>("Api validation requires a URL");
        }

        var url = ReplacePlaceholders(apiData.Url, data);
        var payload = ReplacePlaceholders(apiData.PayloadTemplate, data);

        try
        {
            // Assumed This Response Body for Simplicity --> We can generalize it later
            var response = await PostAsync(
                endpoint: url,
                response: new
                {
                    status = string.Empty,
                    Error = string.Empty
                },
                headers: apiData.Headers,
                payload: payload == null ? null : JsonSerializer.Deserialize<object>(payload));

            return response.status == "200"
                ? Result.Ok(JsonSerializer.Serialize(response))
                : Result.Fail<string>(response.Error);
        }
        catch (Exception e)
        {
            return Result.Fail<string>($"Error While Calling External url: {url} : {e.Message}");
        }
    }

    //Here I handled a simple payload template where variables will be like this "{{processId}}"
    private static string ReplacePlaceholders(string template, Dictionary<string, string> values)
    {
        if (string.IsNullOrEmpty(template))
            return template;

        if (values == null || values.Count == 0)
            return template;

        return Regex.Replace(template, @"\{\{(\w+)\}\}", match =>
        {
            var key = match.Groups[1].Value;
                
            if (values.TryGetValue(key, out var value))
            {
                return value;
            }
            return match.Value;
        });
    }
    

}