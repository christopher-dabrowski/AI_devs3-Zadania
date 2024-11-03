using System.Net.Http.Json;
using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Models;
using Microsoft.Extensions.Options;

namespace Common.AiDevsApi.Services;

public class AiDevsApiService : IAiDevsApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string VerifyEndpoint = "/verify";

    public AiDevsApiService(
        HttpClient httpClient,
        IOptions<AiDevsApiOptions> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey ?? throw new ArgumentNullException(nameof(options.Value.ApiKey));

        // Set the base address if provided in options
        if (!string.IsNullOrEmpty(options.Value.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        }
    }

    public async Task<ApiResponse> VerifyTaskAnswerAsync<TAnswer>(
        TaskAnswer<TAnswer> answer,
        CancellationToken cancellationToken = default)
    {
        var request = new TaskRequest<TAnswer>
        {
            Task = answer.Task,
            Answer = answer.Answer,
            ApiKey = _apiKey
        };

        var response = await _httpClient.PostAsJsonAsync(VerifyEndpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken);
        return result ?? throw new InvalidOperationException("Response was null");
    }
}