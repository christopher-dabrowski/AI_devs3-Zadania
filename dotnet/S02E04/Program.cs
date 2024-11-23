using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Extensions;
using Common.AiDevsApi.Models;
using Common.Cache.Contracts;
using Common.Cache.Extensions;
using Common.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using S02E04;
using S02E04.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi()
    .AddOpenAIClient()
    .AddOpenAIChatClient()
    .AddFileCacheService();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var directory = Path.Combine(AppContext.BaseDirectory, "Resources", "pliki_z_fabryki");
var categorized = await GetTextContents(directory)
    .SelectAwait(async (file) => (file.Name, Category: await AssignCategory(file.Content)))
    .ToLookupAsync(file => file.Category, file => file.Name);

var people = categorized[Category.People].OrderBy(file => file).ToArray();
var hardware = categorized[Category.Hardware].OrderBy(file => file).ToArray();

var taskAnswer = new TaskAnswer<TaskAnswerData>
{
    Task = "kategorie",
    Answer = new TaskAnswerData { People = people, Hardware = hardware },
};
Console.WriteLine(JsonSerializer.Serialize(taskAnswer));
Console.WriteLine();

var response = await sp.GetRequiredService<IAiDevsApiService>().VerifyTaskAnswerAsync(taskAnswer);
Console.WriteLine(JsonSerializer.Serialize(response));

async IAsyncEnumerable<(string Name, string Content)> GetTextContents(string directory)
{
    var textFiles = Directory.EnumerateFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
    foreach (var textFile in textFiles)
    {
        var content = await File.ReadAllTextAsync(textFile);
        yield return (Path.GetFileName(textFile), content);
    }

    var cacheService = sp.GetRequiredService<ICacheService>();

    var audioFiles = Directory.EnumerateFiles(directory, "*.mp3", SearchOption.TopDirectoryOnly);
    foreach (var audioFile in audioFiles)
    {
        var cacheKey = Path.GetFileName(audioFile);
        var content = await cacheService.GetAsync(cacheKey);
        if (content is null)
        {
            content = await GetAudioContent(audioFile);
            await cacheService.SetAsync(cacheKey, content);
        }

        yield return (Path.GetFileName(audioFile), content);
    }

    var imageFiles = Directory.EnumerateFiles(directory, "*.png", SearchOption.TopDirectoryOnly);
    foreach (var imageFile in imageFiles)
    {
        var cacheKey = Path.GetFileName(imageFile);
        var content = await cacheService.GetAsync(cacheKey);
        if (content is null)
        {
            content = await GetImageContent(imageFile);
            await cacheService.SetAsync(cacheKey, content);
        }

        yield return (Path.GetFileName(imageFile), content);
    }
}

async Task<string> GetAudioContent(string filePath)
{
    var audioClient = sp.GetRequiredService<OpenAIClient>().GetAudioClient("whisper-1");

    AudioTranscriptionOptions options = new()
    {
        ResponseFormat = AudioTranscriptionFormat.Simple,
    };

    var transcription = await audioClient.TranscribeAudioAsync(filePath, options);
    return transcription.Value.Text;
}

async Task<string> GetImageContent(string filePath)
{
    var chatClient = sp.GetRequiredService<ChatClient>();

    await using Stream imageStream = File.OpenRead(filePath);
    var imageBytes = BinaryData.FromStream(imageStream);

    List<ChatMessage> messages =
        [
            new SystemChatMessage(Prompts.SystemExtractNoteText),
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart(Prompts.UserExtractNoteText),
                ChatMessageContentPart.CreateImagePart(imageBytes, MimeTypes.GetMimeType(filePath))),
        ];

    var response = await chatClient.CompleteChatAsync(messages);
    return response.Value.Content[0].Text;
}

async Task<Category> AssignCategory(string text)
{
    var chatClient = sp.GetRequiredService<ChatClient>();

    List<ChatMessage> messages =
        [
            new SystemChatMessage(Prompts.Categorize),
            new UserChatMessage(text),
        ];

    var response = await chatClient.CompleteChatAsync(messages);
    var responseText = response.Value.Content[0].Text;
    return Enum.Parse<Category>(responseText, ignoreCase: true);
}
