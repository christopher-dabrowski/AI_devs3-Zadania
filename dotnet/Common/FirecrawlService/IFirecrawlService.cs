namespace Common.FirecrawlService;

public interface IFirecrawlService
{
    Task<FirecrawlResponse> ScrapeAsync(FirecrawlRequest request, CancellationToken cancellationToken = default);
}
