using System.Text.Json.Serialization;

namespace Common.AiDevsApi.Models;

public class TaskRequest<T> : TaskAnswer<T>
{
    [JsonPropertyName("apikey")]
    public required string ApiKey { get; set; }
}