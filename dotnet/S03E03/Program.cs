using System.Net.Http.Json;
using S03E03;
using S03E03.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddS03E03()
    .Configure<S03E03Options>(
        builder.Configuration.GetSection(nameof(S03E03Options)));

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

// Main logic will go here 

async Task<string> RunDbCommand(string command)
{
    var aiDevsOptions = sp.GetRequiredService<IOptions<AiDevsApiOptions>>().Value;
    var taskOptions = sp.GetRequiredService<IOptions<S03E03Options>>().Value;

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
