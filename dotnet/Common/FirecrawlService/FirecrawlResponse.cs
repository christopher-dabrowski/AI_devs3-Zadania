using System.Text.Json.Serialization;

namespace Common.FirecrawlService;

public record FirecrawlResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    public required FirecrawlData Data { get; init; }
}

public record FirecrawlData
{
    [JsonPropertyName("markdown")]
    public string? Markdown { get; init; }

    [JsonPropertyName("html")]
    public string? Html { get; init; }

    [JsonPropertyName("rawHtml")]
    public string? RawHtml { get; init; }

    [JsonPropertyName("links")]
    public string[]? Links { get; init; }

    [JsonPropertyName("screenshot")]
    public string? Screenshot { get; init; }

    [JsonPropertyName("extract")]
    public object? Extract { get; init; }
}
