using System.Text.Json.Serialization;

namespace S03E04.Models;

public record SearchRequest
{
    [JsonPropertyName("apikey")]
    public required string ApiKey { get; init; }

    [JsonPropertyName("query")]
    public required string Query { get; init; }
}
