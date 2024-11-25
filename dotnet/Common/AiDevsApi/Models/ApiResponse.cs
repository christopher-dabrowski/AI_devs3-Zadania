using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.AiDevsApi.Models;

public record ApiResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    public bool IsSuccess => Code == 0;

    public override string ToString() => JsonSerializer.Serialize(this);
}
