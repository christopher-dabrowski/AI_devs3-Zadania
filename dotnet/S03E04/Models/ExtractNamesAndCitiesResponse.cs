using System.Text.Json.Serialization;

namespace S03E04.Models;

public record ExtractNamesAndCitiesResponse
{
    [JsonPropertyName("names")]
    public List<string> Names { get; init; } = new();

    [JsonPropertyName("cities")]
    public List<string> Cities { get; init; } = new();
}
