using OpenAI.Embeddings;
using static MoreLinq.Extensions.EquiZipExtension;
using S03E02;
using Qdrant.Client;
using Qdrant.Client.Grpc;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS03E02();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var openAiClient = sp.GetRequiredService<OpenAIClient>();
var embeddingModel = "text-embedding-3-small";
var embeddingsClient = openAiClient.GetEmbeddingClient(embeddingModel);

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

var quadrant = new QdrantClient("localhost", 6334);

var collectionName = "weapons_tests";
if (await quadrant.CollectionExistsAsync(collectionName))
    await quadrant.DeleteCollectionAsync(collectionName);
await quadrant.CreateCollectionAsync(collectionName, vectorsConfig: new VectorParams
{
    Size = EmbeddingLengthByModel[embeddingModel],
    Distance = Distance.Cosine
});



partial class Program
{
    static IReadOnlyDictionary<string, ulong> EmbeddingLengthByModel = new Dictionary<string, ulong>
    {
        { "text-embedding-3-small", 1536UL },
        { "text-embedding-3-large", 3072UL },
    };
}
