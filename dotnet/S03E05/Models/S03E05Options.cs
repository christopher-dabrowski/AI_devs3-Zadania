using System.ComponentModel.DataAnnotations;

namespace S03E05.Models;

public class S03E05Options
{
    public const string SectionName = "S03E05";

    [Required]
    public string DatabaseApiUrl { get; init; } = string.Empty;

    [Required]
    public string Neo4jUri { get; init; } = "bolt://localhost:7687";

    [Required]
    public string Neo4jUser { get; init; } = string.Empty;

    [Required]
    public string Neo4jPassword { get; init; } = string.Empty;
}
