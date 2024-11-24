namespace Common.FirecrawlService;

public record FirecrawlResponse(
    bool Success,
    FirecrawlData Data);

public record FirecrawlData(
    string? Markdown,
    string? Html,
    string? RawHtml,
    string[]? Links,
    string? Screenshot,
    object? Extract);
