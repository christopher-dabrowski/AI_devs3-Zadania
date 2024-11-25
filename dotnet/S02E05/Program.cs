using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Models;
using Common.Cache.Contracts;
using Common.FirecrawlService;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using S02E05;
using S02E05.Models;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS02E05();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var article = await GetArticleContent();

var images = MarkdownParser.FindImages(article).ToList();
var modifiedArticle = await ReplaceImagesWithDescriptions(article);
modifiedArticle = await ReplaceAudioWithTranscriptions(modifiedArticle);

var questions = await FetchQuestions();
var answers = await AnswerQuestions(questions, modifiedArticle);

Console.WriteLine(answers);

var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
var taskAnswer = new TaskAnswer<JsonObject>
{
    Task = "arxiv",
    Answer = answers,
};
var response = await aiDevsApiService.VerifyTaskAnswerAsync(taskAnswer);

Console.WriteLine(response);

async Task<JsonObject> AnswerQuestions(string questions, string modifiedArticle)
{
    var chatClient = sp.GetRequiredService<ChatClient>();

    List<ChatMessage> messages =
    [
        new SystemChatMessage(Prompts.AnswerQuestionsBasedOnKnowledge(modifiedArticle)),
        new UserChatMessage(ChatMessageContentPart.CreateTextPart(questions))
    ];

    var response = await chatClient.CompleteChatAsync(messages);
    return JsonNode.Parse(response.Value.Content[0].Text) as JsonObject
        ?? throw new InvalidOperationException("Answer is not a JSON object");
}

async Task<string> FetchQuestions()
{
    var aiDevsHttpClient = sp.GetRequiredService<IAiDevsApiService>().HttpClient;
    var aiDevsApiKey = sp.GetRequiredService<IOptions<AiDevsApiOptions>>().Value.ApiKey;

    var response = await aiDevsHttpClient.GetAsync($"data/{aiDevsApiKey}/arxiv.txt");
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    return content;
}

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
        </image>

        """;

        content = content[..image.StartIndex] + structuredDescription + content[image.EndIndex..];
    }

    return content;
}

async Task<string> ReplaceAudioWithTranscriptions(string content)
{
    var audioFiles = MarkdownParser.FindAudioFiles(content).ToImmutableArray();
    foreach (var audio in audioFiles.OrderByDescending(x => x.StartIndex))
    {
        var audioBytes = await DownloadAudio(audio.AudioUrl);
        var transcription = await TranscribeAudio(audioBytes, Path.GetFileName(audio.AudioUrl));

        var structuredTranscription =
        $"""
        <audio name="{audio.Description}">
        {transcription}
        </audio>

        """;

        content = content[..audio.StartIndex] + structuredTranscription + content[audio.EndIndex..];
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

async Task<byte[]> DownloadAudio(string url)
{
    var cacheService = sp.GetRequiredService<ICacheService>();
    var cacheKey = $"audio_{url.Replace("/", "_")}";

    var cachedAudio = await cacheService.GetAsyncBytes(cacheKey);
    if (cachedAudio is not null)
        return cachedAudio;

    var taskOptions = sp.GetRequiredService<IOptions<S02E05Options>>().Value;
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    httpClient.BaseAddress = new Uri(taskOptions.DataUrl);

    var response = await httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var audioBytes = await response.Content.ReadAsByteArrayAsync();

    await cacheService.SetAsyncBytes(cacheKey, audioBytes);
    return audioBytes;
}

async Task<string> TranscribeAudio(byte[] audioBytes, string audioName)
{
    var cacheService = sp.GetRequiredService<ICacheService>();
    var cacheKey = $"transcription_{audioName}";

    var cachedTranscription = await cacheService.GetAsync(cacheKey);
    if (cachedTranscription is not null)
        return cachedTranscription;

    var audioClient = sp.GetRequiredService<OpenAIClient>().GetAudioClient("whisper-1");

    AudioTranscriptionOptions options = new()
    {
        ResponseFormat = AudioTranscriptionFormat.Simple,
        Language = "pl",
    };

    using var audioStream = new MemoryStream(audioBytes);

    var response = await audioClient.TranscribeAudioAsync(audioStream, audioName, options);
    var transcription = response.Value.Text;

    await cacheService.SetAsync(cacheKey, transcription);
    return transcription;
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
