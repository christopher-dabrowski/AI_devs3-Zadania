using OpenAI.Embeddings;
using static MoreLinq.Extensions.EquiZipExtension;
using S03E02;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS03E02();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var openAiClient = sp.GetRequiredService<OpenAIClient>();
var embeddingsClient = openAiClient.GetEmbeddingClient("text-embedding-3-small");

var weaponsTestFiles = await
    Directory.EnumerateFiles("Resources/weapons_tests", "*.txt")
    .Select(async file => (Date: Path.GetFileNameWithoutExtension(file), Description: await File.ReadAllTextAsync(file)))
    .ToArray()
    .ToAsyncEnumerable()
    .SelectAwait(async x => ((await x).Date, (await x).Description))
    .ToArrayAsync();

var descriptions = weaponsTestFiles.Select(x => x.Description);

var embeddings = await embeddingsClient.GenerateEmbeddingsAsync(descriptions);

var weaponsTestEmbeddings = weaponsTestFiles
    .EquiZip(embeddings.Value, (x, y) => (x.Date, x.Description, Embedding: y))
    .ToArray();

