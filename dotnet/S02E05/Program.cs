using Common.AiDevsApi.Extensions;
using Common.Cache.Contracts;
using Common.Cache.Extensions;
using Common.FirecrawlService;
using Common.FirecrawlService.Extensions;
using S02E05;
using S02E05.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS02E05();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var article = await GetArticleContent(sp);

Console.WriteLine(article);

static async Task<string> GetArticleContent(IServiceProvider sp)
{
    var cacheService = sp.GetRequiredService<ICacheService>();
    var cacheKey = $"article";

    var cachedContent = await cacheService.GetAsync(cacheKey);
    if (cachedContent is not null)
        return cachedContent;

    var firecrawlService = sp.GetRequiredService<IFirecrawlService>();
    var taskOptions = sp.GetRequiredService<IOptions<S02E05Options>>().Value;

    var result = await firecrawlService.ScrapeAsync(new FirecrawlRequest
    {
        Url = taskOptions.ArticleUrl,
        Formats = ["markdown"],
    });

    var articleMarkdown = result.Data.Markdown
        ?? throw new InvalidOperationException("Article markdown is null");

    await cacheService.SetAsync(cacheKey, articleMarkdown);
    return articleMarkdown;
}
