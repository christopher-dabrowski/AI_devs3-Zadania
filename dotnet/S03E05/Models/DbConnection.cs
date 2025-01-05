namespace S03E05.Models;

using System.Text.Json.Serialization;

public record DbConnection
{
    [JsonPropertyName("user1_id")]
    public required string User1Id { get; set; }

    [JsonPropertyName("user2_id")]
    public required string User2Id { get; set; }
}
