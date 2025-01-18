namespace S04E04;

using System.Text.Json.Serialization;

public sealed record FlightDescriptionRequest
{
    [JsonPropertyName("instruction")]
    public required string Instruction { get; init; }
}
