using System.Text;
using System.Text.Json;

namespace Common.FirecrawlService;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

public class FirecrawlService : IFirecrawlService
{
    private readonly HttpClient _httpClient;

    public FirecrawlService(HttpClient httpClient, IOptions<FirecrawlOptions> options)
    {
        _httpClient = httpClient;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.ApiKey);
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
    }

    public async Task<FirecrawlResponse> ScrapeAsync(FirecrawlRequest request, CancellationToken cancellationToken = default)
    {
        var serializationOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var serializedRequest = JsonSerializer.Serialize(request, serializationOptions);
        var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/v1/scrape", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<FirecrawlResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}
