namespace Common.FirecrawlService;

public record FirecrawlRequest(
    string Url,
    string[]? Formats = null,
    bool OnlyMainContent = true,
    string[]? IncludeTags = null,
    string[]? ExcludeTags = null,
    Dictionary<string, string>? Headers = null,
    int? WaitFor = null,
    bool? Mobile = null,
    bool? SkipTlsVerification = null,
    int? Timeout = null,
    LocationSettings? Location = null,
    bool? RemoveBase64Images = null);

public record LocationSettings(
    string Country,
    string[]? Languages = null);
