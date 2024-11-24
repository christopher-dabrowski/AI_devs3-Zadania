using System.ComponentModel.DataAnnotations;

namespace S02E05.Models;

public class S02E05Options
{
    public const string SectionName = "S02E05";

    [Required]
    public string DataUrl { get; init; } = string.Empty;

    public string ArticleUrl => DataUrl.TrimEnd('/') + "/arxiv-draft.html";
}
