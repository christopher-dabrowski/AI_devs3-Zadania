using System.Text.Json.Serialization;

namespace Common.AiDevsApi.Models;

public class TaskAnswer<T>
{
    [JsonPropertyName("task")]
    public required string Task { get; set; }

    [JsonPropertyName("answer")]
    public required T Answer { get; set; }
}

// For convenience, add a non-generic version for string answers
public class TaskAnswer : TaskAnswer<string>
{
}