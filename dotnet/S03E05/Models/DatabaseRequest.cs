using System.Text.Json.Serialization;

namespace S03E05.Models;

public record DatabaseRequest
{
    [JsonPropertyName("task")]
    public string Task { get; } = "database";

    [JsonPropertyName("apikey")]
    public required string ApiKey { get; init; }

    [JsonPropertyName("query")]
    public required string Query { get; init; }
}
