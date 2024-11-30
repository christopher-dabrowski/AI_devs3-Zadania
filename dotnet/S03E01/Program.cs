using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Models;
using OpenAI.Chat;
using S03E01;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS03E01();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var facts = await GetFacts().ToArrayAsync();
var reportFiles = ListReportFiles();

var systemPrompt = Prompts.KeywordGenerationSystem(facts);
Console.WriteLine(systemPrompt);
Environment.Exit(0);

var fileNamesAndKeywords = await reportFiles
    .Select(async reportFile =>
        (
            FileName: Path.GetFileName(reportFile),
            Keywords: await GenerateKeywords(reportFile, facts)
        )
    )
    .ToArray()
    .ToAsyncEnumerable()
    .ToDictionaryAwaitAsync(
        async x => (await x).FileName,
        async x => (await x).Keywords
    );

var taskAnswer = new TaskAnswer<IDictionary<string, string>>
{
    Task = "dokumenty",
    Answer = fileNamesAndKeywords
};
Console.WriteLine(JsonSerializer.Serialize(taskAnswer));

var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
var verificationResult = await aiDevsApiService.VerifyTaskAnswerAsync(taskAnswer);
Console.WriteLine(JsonSerializer.Serialize(verificationResult));

async Task<string> GenerateKeywords(string reportFilePath, IEnumerable<string> facts)
{
    var chatClient = sp.GetRequiredService<ChatClient>();

    var reportText = await File.ReadAllTextAsync(reportFilePath);
    ChatMessage[] chatMessages =
    [
        new SystemChatMessage(Prompts.KeywordGenerationSystem(facts)),
        new UserChatMessage(reportText)
    ];

    var response = await chatClient.CompleteChatAsync(chatMessages);
    return response.Value.Content[0].Text;
}

IEnumerable<string> ListReportFiles()
{
    var factoryPath = Path.Combine(".", "Resources", "pliki_z_fabryki");
    return Directory.GetFiles(factoryPath, "*.txt");
}

IAsyncEnumerable<string> GetFacts()
{
    var factsPath = Path.Combine(".", "Resources", "pliki_z_fabryki", "facts");
    var factFiles = Directory.GetFiles(factsPath, "*.txt");
    return factFiles
        .Select(async file => await File.ReadAllTextAsync(file))
        .ToArray()
        .ToAsyncEnumerable()
        .SelectAwait(async (fact) => await fact)
        .Where(fact => !fact.StartsWith("entry deleted", StringComparison.OrdinalIgnoreCase));
}
