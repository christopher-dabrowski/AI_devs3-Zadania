using Common.AiDevsApi.Extensions;
using Common.Cache.Contracts;
using Common.Cache.Extensions;
using Common.FirecrawlService;
using Common.FirecrawlService.Extensions;
using OpenAI.Chat;
using S02E05;
using S02E05.Models;
using System.Collections.Generic;
using System.Collections.Immutable;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS02E05();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var article = await GetArticleContent();

var images = MarkdownParser.FindImages(article).ToList();
var modifiedArticle = await ReplaceImagesWithDescriptions(article);
Console.WriteLine(modifiedArticle);

async Task<string> ReplaceImagesWithDescriptions(string content)
{
    var images = MarkdownParser.FindImages(content).ToImmutableArray();
    foreach (var image in images.OrderByDescending(x => x.StartIndex))
    {
        var imageBytes = await DownloadImage(image.ImageUrl);
        var description = await DescribeImage(imageBytes, Path.GetFileName(image.ImageUrl));

        var structuredDescription =
        $"""
        <image>
        {description}
        </image>\n
        """;

        content = content[..image.StartIndex] + structuredDescription + content[image.EndIndex..];
    }

    return content;
}

async Task<byte[]> DownloadImage(string url)
{
    var cacheService = sp.GetRequiredService<ICacheService>();
    var cacheKey = url.Replace("/", "_");

    var cachedImage = await cacheService.GetAsyncBytes(cacheKey);
    if (cachedImage is not null)
        return cachedImage;

    var taskOptions = sp.GetRequiredService<IOptions<S02E05Options>>().Value;
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    httpClient.BaseAddress = new Uri(taskOptions.DataUrl);

    var response = await httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var imageBytes = await response.Content.ReadAsByteArrayAsync();

    await cacheService.SetAsyncBytes(cacheKey, imageBytes);
    return imageBytes;
}

async Task<string> DescribeImage(byte[] imageBytes, string imageName, string mimeType = "image/png")
{
    var cacheService = sp.GetRequiredService<ICacheService>();
    var cacheKey = $"description_{imageName}";

    var cachedDescription = await cacheService.GetAsync(cacheKey);
    if (cachedDescription is not null)
        return cachedDescription;

    var chatClient = sp.GetRequiredService<ChatClient>();
    var binaryData = BinaryData.FromBytes(imageBytes);

    List<ChatMessage> messages =
    [
        new SystemChatMessage(Prompts.SystemDescribeImage),
        new UserChatMessage(
            ChatMessageContentPart.CreateTextPart(Prompts.UserDescribeImage),
            ChatMessageContentPart.CreateImagePart(binaryData, mimeType))
    ];

    var response = await chatClient.CompleteChatAsync(messages);
    var description = response.Value.Content[0].Text;

    await cacheService.SetAsync(cacheKey, description);
    return description;
}

async Task<string> GetArticleContent()
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
