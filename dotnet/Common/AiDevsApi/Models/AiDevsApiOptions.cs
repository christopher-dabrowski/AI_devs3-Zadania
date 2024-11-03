using System.ComponentModel.DataAnnotations;

namespace Common.AiDevsApi.Models;

public class AiDevsApiOptions
{
    public const string SectionName = "AiDevsApi";

    [Required]
    public required string ApiKey { get; set; }

    [Required]
    public required string BaseUrl { get; init; }
}
