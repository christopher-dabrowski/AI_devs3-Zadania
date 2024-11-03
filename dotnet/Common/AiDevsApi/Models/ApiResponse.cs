using System.Text.Json.Serialization;

namespace Common.AiDevsApi.Models;

public class ApiResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    public bool IsSuccess => Code == 0;
}
