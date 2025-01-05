using System.Net.Http.Json;
using Common.AiDevsApi.Models;
using S03E05;
using S03E05.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddS03E05();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var dbUsers = await ListUsers();
var dbConnections = await ListConnections();

Console.WriteLine(JsonSerializer.Serialize(dbUsers));
Console.WriteLine(JsonSerializer.Serialize(dbConnections));


// Add task-specific logic here

// var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
// var taskResponse = new TaskAnswer<object>()
// {
//     Task = "task_name",
//     Answer = null // Replace with actual answer
// };

// var taskResponseResult = await aiDevsApiService.VerifyTaskAnswerAsync(taskResponse);
// Console.WriteLine(JsonSerializer.Serialize(taskResponseResult));

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
