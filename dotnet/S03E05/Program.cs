using System.Net.Http.Json;
using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Models;
using Neo4j.Driver;
using S03E05;
using S03E05.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddS03E05();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var neo4jDriver = sp.GetRequiredService<IDriver>();

const string GraphDbName = "neo4j";

var dbUsers = await ListUsers();
await LoadUsersToGraphDb(dbUsers);
Console.WriteLine("Users loaded");

var dbConnections = await ListConnections();
await LoadConnectionsToGraphDb(dbConnections);
Console.WriteLine("Connections loaded");

const string startName = "Rafał";
const string endName = "Barbara";
const string ShortestPathCypher =
    """
    MATCH p = SHORTEST 1 (r:User {username: $startName})-[:KNOWS]-+(b:User {username: $endName})
    RETURN [n IN nodes(p) | n.username] AS names
    """;
var shortestPathResult = await neo4jDriver
    .ExecutableQuery(ShortestPathCypher)
    .WithParameters(new { startName, endName })
    .ExecuteAsync();

var shortestPath = shortestPathResult
    .Result
    .Select(r => r["names"].As<List<object>>().Select(s => s.ToString()))
    .Single();
Console.WriteLine(JsonSerializer.Serialize(shortestPath));

var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
var taskResponse = new TaskAnswer<object>()
{
    Task = "connections",
    Answer = string.Join(", ", shortestPath)
};

var taskResponseResult = await aiDevsApiService.VerifyTaskAnswerAsync(taskResponse);
Console.WriteLine(JsonSerializer.Serialize(taskResponseResult));

async Task LoadUsersToGraphDb(IEnumerable<DbUser> dbUsers)
{
    await using var session = neo4jDriver.AsyncSession(o => o.WithDatabase(GraphDbName));

    const string Cypher = "MERGE (u:User {id: $id, username: $username})";
    foreach (var dbUser in dbUsers)
    {
        var result = await session.ExecuteWriteAsync(x => x.RunAsync(
            Cypher,
            new { id = dbUser.Id, username = dbUser.Username }));
    }
}

async Task LoadConnectionsToGraphDb(IEnumerable<DbConnection> connections)
{
    await using var session = neo4jDriver.AsyncSession(o => o.WithDatabase(GraphDbName));

    const string Cypher = """
        MATCH (u1:User {id: $user1Id})
        MATCH (u2:User {id: $user2Id})
        MERGE (u1)-[r:KNOWS]->(u2)
        """;

    foreach (var connection in connections)
    {
        var result = await session.ExecuteWriteAsync(x => x.RunAsync(
            Cypher,
            new { user1Id = connection.User1Id, user2Id = connection.User2Id }));
    }
}

async Task<IReadOnlyList<DbUser>> ListUsers()
{
    const string Command = "SELECT * FROM users";
    var dbResponse = await RunDbCommand(Command);

    var parsedResponse = JsonSerializer.Deserialize<DbResponse<DbUser>>(dbResponse)
        ?? throw new InvalidOperationException("Invalid API response");
    return parsedResponse.Reply;
}

async Task<IReadOnlyList<DbConnection>> ListConnections()
{
    const string Command = "SELECT * FROM connections";
    var dbResponse = await RunDbCommand(Command);

    var parsedResponse = JsonSerializer.Deserialize<DbResponse<DbConnection>>(dbResponse)
        ?? throw new InvalidOperationException("Invalid API response");
    return parsedResponse.Reply;
}

async Task<string> RunDbCommand(string command)
{
    var aiDevsOptions = sp.GetRequiredService<IOptions<AiDevsApiOptions>>().Value;
    var taskOptions = sp.GetRequiredService<IOptions<S03E05Options>>().Value;

    var dbRequest = new DatabaseRequest
    {
        ApiKey = aiDevsOptions.ApiKey,
        Query = command,
    };

    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();

    var response = await httpClient.PostAsJsonAsync(taskOptions.DatabaseApiUrl, dbRequest);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsStringAsync();
}
