using System.ComponentModel.DataAnnotations;

namespace Common.FirecrawlService;

public class FirecrawlOptions
{
    public const string SectionName = "Firecrawl";

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string BaseUrl { get; init; } = "https://api.firecrawl.dev/";
}
