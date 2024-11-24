using System.ComponentModel.DataAnnotations;

namespace S02E05.Models;

public class S02E05Options
{
    public const string SectionName = "S02E05";

    [Required]
    public string ArticleUrl { get; init; } = string.Empty;
}
