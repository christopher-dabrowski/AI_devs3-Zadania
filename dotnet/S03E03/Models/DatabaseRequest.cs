namespace S03E03.Models;

public record DatabaseRequest
{
    public string Task { get; } = "database";

    public required string ApiKey { get; init; }

    public required string Query { get; init; }
}
