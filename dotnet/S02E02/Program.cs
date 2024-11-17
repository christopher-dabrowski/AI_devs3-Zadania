using Common.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using S02E02;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOpenAIChatClient();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var chatClient = sp.GetRequiredService<ChatClient>();

var mapsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Maps");
var mapFiles = Directory.GetFiles(mapsDirectory, "*.png");

var mapDescriptions = new Dictionary<string, string>();
foreach (var mapFile in mapFiles)
{
    using Stream imageStream = File.OpenRead(mapFile);
    BinaryData imageBytes = BinaryData.FromStream(imageStream);

    List<ChatMessage> messages =
        [
            new SystemChatMessage(Prompts.DescribeMapSystemPrompt),
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart(Prompts.DescribeMapUserPrompt),
                ChatMessageContentPart.CreateImagePart(imageBytes, MimeTypes.GetMimeType(mapFile))),
        ];

    ChatCompletion completion = await chatClient.CompleteChatAsync(messages);

    mapDescriptions[Path.GetFileName(mapFile)] = completion.Content[0].Text;

}

foreach (var (mapFile, description) in mapDescriptions)
{
    var descriptionsLocation = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Descriptions");
    await File.WriteAllTextAsync(Path.Combine(descriptionsLocation, $"{Path.GetFileName(mapFile)}.txt"), description);
}

// One of the maps is from the wrong city
foreach (var combination in CombinationsWithOneExcluded(mapDescriptions))
{
    var userPrompt = string.Join("\n\n", combination.Select(m => $"{m.Key}: {m.Value}"));

    List<ChatMessage> messages =
        [
            new SystemChatMessage(Prompts.DetectCitySystemPrompt),
            new UserChatMessage(userPrompt),
        ];

    ChatCompletion completion = await chatClient.CompleteChatAsync(messages);

    Console.WriteLine(completion.Content[0].Text);
    Console.WriteLine();
}

static IEnumerable<IReadOnlyDictionary<string, string>> CombinationsWithOneExcluded(IReadOnlyDictionary<string, string> mapDescriptions)
{
    foreach (var mapDescription in mapDescriptions)
    {
        var remainingMaps = mapDescriptions.Where(m => m.Key != mapDescription.Key);
        yield return new Dictionary<string, string>(remainingMaps);
    }
}
