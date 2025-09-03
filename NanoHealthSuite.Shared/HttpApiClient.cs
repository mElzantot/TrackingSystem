using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NanoHealthSuite.TrackingSystem.Shared;

public class HttpApiClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<TResponse> GetAsync<TResponse>(
        string endpoint,
        TResponse response,
        string authorizationScheme = "Bearer",
        Dictionary<string, string> headers = null,
        JsonSerializerOptions serializerOptions = null)
    {
        var stringResponse = await GetRawAsync(
            endpoint: endpoint,
            authorizationScheme: authorizationScheme,
            headers: headers,
            serializerOptions: serializerOptions);
            
        return JsonSerializer.Deserialize<TResponse>(stringResponse, _jsonSerializerOptions);
    }

    private async Task<string> GetRawAsync(
        string endpoint,
        string authorizationScheme = "Bearer",
        Dictionary<string, string> headers = null,
        JsonSerializerOptions serializerOptions = null)
    {
        var response = await SendRequestAsync(
            method: HttpMethod.Get,
            endpoint: endpoint,
            authorizationScheme: authorizationScheme,
            headers: headers,
            serializerOptions: serializerOptions);

        var stringResponse = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(stringResponse);
        }

        return stringResponse;
    }

    public async Task<TResponse> PostAsync<TResponse>(
        string endpoint,
        TResponse response,
        string authorizationScheme = "Bearer",
        object payload = null,
        Dictionary<string, string> headers = null,
        JsonSerializerOptions serializerOptions = null)
    {
        var stringResponse = await PostRawAsync(
            endpoint: endpoint,
            authorizationScheme: authorizationScheme,
            payload: payload,
            headers: headers,
            serializerOptions: serializerOptions);
            
        return JsonSerializer.Deserialize<TResponse>(stringResponse, _jsonSerializerOptions);
    }

    private async Task<string> PostRawAsync(
        string endpoint,
        string authorizationScheme = "Bearer",
        object payload = null,
        Dictionary<string, string> headers = null,
        JsonSerializerOptions serializerOptions = null)
    {
        var response = await SendRequestAsync(
            method: HttpMethod.Post,
            endpoint: endpoint,
            authorizationScheme: authorizationScheme,
            payload: payload,
            headers: headers,
            serializerOptions: serializerOptions);
        
        var stringResponse = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(message: stringResponse, inner: null, statusCode: response.StatusCode);
        }

        return stringResponse;
    }

    public async Task DeleteAsync(
        string endpoint,
        string authorizationScheme = "Bearer",
        Dictionary<string, string> headers = null)
    {
        
        var response = await SendRequestAsync(
            method: HttpMethod.Delete,
            endpoint: endpoint,
            authorizationScheme: authorizationScheme,
            headers: headers);
        
        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod method,
        string endpoint, 
        string authorizationScheme,
        object payload = null, 
        Dictionary<string, string> headers = null,
        JsonSerializerOptions serializerOptions = null)
    {
        var requestReference = $"req__{Guid.NewGuid():N}";
        
        using var request = new HttpRequestMessage(method, endpoint);

        if (payload != null)
        {
            if (headers?.Values.Contains("application/x-www-form-urlencoded") ?? false)
            {
                request.Content = new FormUrlEncodedContent((List<KeyValuePair<string, string>>)payload);
            }
            else
            {
                var serializedRequest = JsonSerializer.Serialize(payload ?? "", serializerOptions).Replace("@return", "");
                request.Content = new StringContent(serializedRequest, Encoding.UTF8, mediaType: "application/json");
            }
        }

        AddHeaders(request, headers, authorizationScheme);
        var response = await _httpClient.SendAsync(request);
        return response;
    }
    
    private void AddHeaders(HttpRequestMessage request, Dictionary<string, string> headers, string authorizationScheme)
    {
        if (headers == null) return;

        foreach (var header in headers)
        {
            if (header.Key == "Content-Type")
            {
                continue;
            }
            
            if (request.Headers.Contains(header.Key))
            {
                request.Headers.Remove(header.Key);
            }

            if (header.Key == "Authorization" && authorizationScheme == "Bearer")
            { 
                request.Headers.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", header.Value);
            }
            else
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
    }
}
