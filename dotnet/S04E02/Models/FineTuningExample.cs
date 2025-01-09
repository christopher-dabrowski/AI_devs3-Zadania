using System.Text.Json.Serialization;

namespace S04E02.Models;

public record FineTuningExample
{
    [JsonPropertyName("messages")]
    public required List<FineTuningChatMessage> Messages { get; init; }
}

public record FineTuningChatMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required string Content { get; init; }
}