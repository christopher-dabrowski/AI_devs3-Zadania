using System.ComponentModel.DataAnnotations;

namespace Common.AiDevsApi.Models;

public record AiDevsApiOptions
{
    public const string SectionName = "AiDevsApi";

    [Required]
    public required string ApiKey { get; init; }

    [Required]
    public required string BaseUrl { get; init; }
}
