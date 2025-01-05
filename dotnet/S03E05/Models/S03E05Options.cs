using System.ComponentModel.DataAnnotations;

namespace S03E05.Models;

public class S03E05Options
{
    public const string SectionName = "S03E05";

    [Required]
    public string DatabaseApiUrl { get; init; } = string.Empty;
}
