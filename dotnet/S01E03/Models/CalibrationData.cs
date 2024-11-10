using System.Text.Json.Serialization;

namespace S01E03.Models;

public sealed record CalibrationData
{
    [JsonPropertyName("apikey")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("copyright")]
    public string Copyright { get; set; } = string.Empty;

    [JsonPropertyName("test-data")]
    public List<TestData> TestData { get; set; } = new();
}

public sealed record TestData
{
    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("answer")]
    public int Answer { get; set; }

    [JsonPropertyName("test")]
    public TestQuestion? Test { get; set; }
}

public sealed record TestQuestion
{
    [JsonPropertyName("q")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("a")]
    public string Answer { get; set; } = string.Empty;
}
