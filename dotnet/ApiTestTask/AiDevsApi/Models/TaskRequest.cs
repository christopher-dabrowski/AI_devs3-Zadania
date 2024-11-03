using System.Text.Json.Serialization;

namespace AiDevsApi.Models;

public class TaskRequest<T> : TaskAnswer<T>
{
    [JsonPropertyName("apikey")]
    public required string ApiKey { get; set; }
}