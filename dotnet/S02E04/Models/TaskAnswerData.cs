using System.Text.Json.Serialization;

namespace S02E04.Models;

public record TaskAnswerData
{
    [JsonPropertyName("people")]
    public required IEnumerable<string> People { get; init; }

    [JsonPropertyName("hardware")]
    public required IEnumerable<string> Hardware { get; init; }
}
