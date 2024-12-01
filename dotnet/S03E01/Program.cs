using System.Collections.Immutable;
using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Models;
using Common.Cache.Contracts;
using OpenAI.Chat;
using S03E01;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS03E01();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var facts = await GetFacts().ToArrayAsync();

var factsKeywords = await facts
    .Select(async fact => (fact.Id, Keywords: await GenerateKeywords(fact.Id, fact.Fact)))
    .Chunk(4) // Run 4 keywords generation in parallel
    .ToAsyncEnumerable()
    .Select(chunk => Task.WhenAll(chunk))
    .SelectManyAwait(async chunk => (await chunk).ToAsyncEnumerable())
    .ToDictionaryAsync(
        x => x.Id,
        x => x.Keywords
    );

var reportFiles = ListReportFiles().ToImmutableArray();

var reportKeywords = await reportFiles
    .Select(async reportFile => (FileName: Path.GetFileName(reportFile), Keywords: await GenerateKeywords(Path.GetFileName(reportFile), await File.ReadAllTextAsync(reportFile))))
    .Chunk(4) // Run 4 keywords generation in parallel
    .ToAsyncEnumerable()
    .Select(chunk => Task.WhenAll(chunk))
    .SelectManyAwait(async chunk => (await chunk).ToAsyncEnumerable())
    .ToDictionaryAsync(
        x => x.FileName,
        x => x.Keywords
    );

var totalReportKeywords = new Dictionary<string, string>();
foreach (var reportFile in reportFiles)
{
    var fileName = Path.GetFileName(reportFile);
    var keywordsFromReport = reportKeywords[fileName]
        .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .ToHashSet();

    var matchingFacts = await SelectFacts(fileName, await File.ReadAllTextAsync(reportFile), facts);
    var factKeywords = matchingFacts
        .Select(fact => factsKeywords[fact])
        .ToHashSet();

    var totalKeywords = keywordsFromReport.Union(factKeywords).ToHashSet();
    totalReportKeywords[fileName] = string.Join(", ", totalKeywords);
}

var taskAnswer = new TaskAnswer<IDictionary<string, string>>
{
    Task = "dokumenty",
    Answer = totalReportKeywords
};
Console.WriteLine(JsonSerializer.Serialize(taskAnswer));

var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
var verificationResult = await aiDevsApiService.VerifyTaskAnswerAsync(taskAnswer);
Console.WriteLine(JsonSerializer.Serialize(verificationResult));

async Task<IReadOnlyCollection<string>> SelectFacts(string reportName, string report, IEnumerable<(string Id, string Fact)> facts)
{
    var cache = sp.GetRequiredService<ICacheService>();
    var cacheKey = $"select-facts-{reportName}";

    var cachedResult = await cache.GetAsync<IImmutableList<string>>(cacheKey);
    if (cachedResult is not null)
    {
        return cachedResult;
    }

    var chatClient = sp.GetRequiredService<ChatClient>();

    ChatMessage[] chatMessages =
    [
        new SystemChatMessage(Prompts.SelectFactsSystem(facts)),
        new UserChatMessage(report)
    ];

    var response = await chatClient.CompleteChatAsync(chatMessages);
    var fullText = response.Value.Content[0].Text;
    var answerIndex = fullText.IndexOf("ANSWER:", StringComparison.OrdinalIgnoreCase);
    var answerRaw = answerIndex >= 0
        ? fullText[(answerIndex + "ANSWER:".Length)..].Trim()
        : fullText.Trim();
    var answer = JsonSerializer.Deserialize<IImmutableList<string>>(answerRaw) ?? [];
    var selectedFactIds = answer.ToImmutableHashSet();

    var selectedFacts = facts
        .Where(fact => selectedFactIds.Contains(fact.Id))
        .Select(fact => fact.Id)
        .ToImmutableArray();

    await cache.SetAsync(cacheKey, selectedFacts);

    return selectedFacts;
}

async Task<string> GenerateKeywords(string fileName, string text)
{
    var cache = sp.GetRequiredService<ICacheService>();
    var cacheKey = $"generate-keywords-{fileName}";

    var cachedResult = await cache.GetAsync<string>(cacheKey);
    if (cachedResult is not null)
    {
        return cachedResult;
    }

    var chatClient = sp.GetRequiredService<ChatClient>();

    ChatMessage[] chatMessages =
    [
        new SystemChatMessage(Prompts.KeywordGenerationSimpleSystem),
        new UserChatMessage(text)
    ];

    var response = await chatClient.CompleteChatAsync(chatMessages);
    var fullText = response.Value.Content[0].Text;

    var answerIndex = fullText.IndexOf("ANSWER:", StringComparison.OrdinalIgnoreCase);
    var answer = answerIndex >= 0
        ? fullText[(answerIndex + "ANSWER:".Length)..].Trim()
        : fullText.Trim();

    await cache.SetAsync(cacheKey, answer);

    return answer;
}

IEnumerable<string> ListReportFiles()
{
    var factoryPath = Path.Combine(".", "Resources", "pliki_z_fabryki");
    return Directory.GetFiles(factoryPath, "*.txt");
}

IAsyncEnumerable<(string Id, string Fact)> GetFacts()
{
    var factsPath = Path.Combine(".", "Resources", "pliki_z_fabryki", "facts");
    var factFiles = Directory.GetFiles(factsPath, "*.txt");
    return factFiles
        .Select(async file => (Id: Path.GetFileNameWithoutExtension(file), Fact: await File.ReadAllTextAsync(file)))
        .ToArray()
        .ToAsyncEnumerable()
        .SelectAwait(async fact => ((await fact).Id, (await fact).Fact))
        .Where(fact => !fact.Fact.StartsWith("entry deleted", StringComparison.OrdinalIgnoreCase));
}
