using System.ComponentModel.DataAnnotations;

namespace S01E02.XyzApi.Models;

public class XyzApiOptions
{
    public const string SectionName = "XyzApi";

    [Required]
    public required string BaseUrl { get; init; }
}
