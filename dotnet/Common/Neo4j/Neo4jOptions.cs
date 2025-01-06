using System.ComponentModel.DataAnnotations;

namespace Common.OpenAi.Models;

public class Neo4jOptions
{
    public const string SectionName = "Neo4j";

    [Required]
    public string Uri { get; init; } = "bolt://localhost:7687";

    [Required]
    public string User { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
