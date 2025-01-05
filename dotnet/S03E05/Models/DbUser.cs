namespace S03E05.Models;

using System.Text.Json.Serialization;

public record DbUser
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("access_level")]
    public required string AccessLevel { get; set; }

    [JsonPropertyName("is_active")]
    public required string IsActive { get; set; }

    [JsonPropertyName("lastlog")]
    public required DateTime LastLog { get; set; }
}

