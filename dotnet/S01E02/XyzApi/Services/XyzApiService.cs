using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using S01E02.XyzApi.Contracts;
using S01E02.XyzApi.Models;

namespace S01E02.XyzApi.Services;

public class XyzApiService : IXyzApiService
{
    private readonly HttpClient _httpClient;
    private const string VerifyEndpoint = "/verify";

    public XyzApiService(
        HttpClient httpClient,
        IOptions<XyzApiOptions> options)
    {
        _httpClient = httpClient;
    }

    public async Task<XyzMessage> VerifyMessageAsync(
        XyzMessage message,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(VerifyEndpoint, message, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<XyzMessage>(cancellationToken);
        return result ?? throw new InvalidOperationException("Response was null");
    }
}
