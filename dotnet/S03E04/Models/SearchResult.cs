using System.Text.Json.Serialization;

namespace S03E04.Models;

public record SearchResult
{
    [JsonPropertyName("code")]
    public int Code { get; init; } = -1;
    
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}
