using System.Text.Json.Serialization;

namespace Common.FirecrawlService;

public record FirecrawlRequest
{
    [JsonPropertyName("url")]
    public string Url { get; init; }

    [JsonPropertyName("formats")]
    public string[]? Formats { get; init; }

    [JsonPropertyName("onlyMainContent")]
    public bool? OnlyMainContent { get; init; }

    [JsonPropertyName("includeTags")]
    public string[]? IncludeTags { get; init; }

    [JsonPropertyName("excludeTags")]
    public string[]? ExcludeTags { get; init; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; init; }

    [JsonPropertyName("waitFor")]
    public int? WaitFor { get; init; }

    [JsonPropertyName("mobile")]
    public bool? Mobile { get; init; }

    [JsonPropertyName("skipTlsVerification")]
    public bool? SkipTlsVerification { get; init; }

    [JsonPropertyName("timeout")]
    public int? Timeout { get; init; }

    [JsonPropertyName("location")]
    public LocationSettings? Location { get; init; }

    [JsonPropertyName("removeBase64Images")]
    public bool? RemoveBase64Images { get; init; }
}

public record LocationSettings
{
    [JsonPropertyName("country")]
    public required string Country { get; init; }

    [JsonPropertyName("languages")]
    public string[]? Languages { get; init; }
}
