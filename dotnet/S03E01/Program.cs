using System.Linq;
using Common.AiDevsApi.Contracts;
using OpenAI.Chat;
using S03E01;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS03E01();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
var chatClient = sp.GetRequiredService<ChatClient>();
var environment = sp.GetRequiredService<IHostEnvironment>();

var reports = ListReportFiles();
foreach (var report in reports)
{
    Console.WriteLine(Path.GetFileName(report));
}

var facts = await GetFacts().ToArrayAsync();
foreach (var fact in facts)
{
    Console.WriteLine(fact);
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
