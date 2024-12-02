using static MoreLinq.Extensions.EquiZipExtension;
using S03E02;
using Qdrant.Client;
using Qdrant.Client.Grpc;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS03E02();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var embeddingModel = "text-embedding-3-small";
var collectionName = "weapons_tests";

var quadrant = new QdrantClient("localhost", 6334);
var openAiClient = sp.GetRequiredService<OpenAIClient>();
var embeddingsClient = openAiClient.GetEmbeddingClient(embeddingModel);

await FillVectorDatabase();

var question = "W raporcie, z którego dnia znajduje się wzmianka o kradzieży prototypu broni?";
var questionEmbedding = await embeddingsClient.GenerateEmbeddingAsync(question);

var result = await quadrant.SearchAsync(collectionName, questionEmbedding.Value.ToFloats(), limit: 1);
var foundDate = result.First().Payload["date"].StringValue;

var aiDevsService = sp.GetRequiredService<IAiDevsApiService>();
var formattedDate = foundDate.Replace("_", "-");
Console.WriteLine(formattedDate);

var taskAnswer = new TaskAnswer
{
    Task = "wektory",
    Answer = formattedDate
};

var response = await aiDevsService.VerifyTaskAnswerAsync(taskAnswer);
Console.WriteLine(JsonSerializer.Serialize(response));

async Task FillVectorDatabase()
{
    if (await quadrant.CollectionExistsAsync(collectionName))
        return;

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

    await quadrant.CreateCollectionAsync(collectionName, vectorsConfig: new VectorParams
    {
        Size = EmbeddingLengthByModel[embeddingModel],
        Distance = Distance.Cosine
    });

    var points = weaponsTestEmbeddings.Select((x, i) => new PointStruct
    {
        Id = (ulong)i,
        Vectors = x.Embedding.ToFloats().ToArray(),
        Payload =
    {
        ["date"] = x.Date,
    }
    }).ToImmutableArray();

    await quadrant.UpsertAsync(collectionName, points);
}

partial class Program
{
    static IReadOnlyDictionary<string, ulong> EmbeddingLengthByModel = new Dictionary<string, ulong>
    {
        { "text-embedding-3-small", 1536UL },
        { "text-embedding-3-large", 3072UL },
    };
}
