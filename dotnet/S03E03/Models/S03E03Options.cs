namespace S03E03.Models;
using System.ComponentModel.DataAnnotations;

public record S03E03Options
{
    public const string SectionName = "S03E03";

    [Required]
    [Url]
    public string DatabaseApiUrl { get; init; } = string.Empty;
}
