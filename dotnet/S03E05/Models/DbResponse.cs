namespace S03E05.Models;

public record DbResponse<T>
{
    [JsonPropertyName("reply")]
    public required IReadOnlyList<T> Reply { get; set; }

    [JsonPropertyName("error")]
    public required string Error { get; set; }
}
