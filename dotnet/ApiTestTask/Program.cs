using System.Diagnostics;
using System.Text.Json;
using AiDevsApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AiDevsApi.Models;
using ApiTestTask.AiDevsApi.Contracts;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAiDevsApi();
        services.AddHttpClient();
    })
    .Build();

await using var scope = host.Services.CreateAsyncScope();
var serviceProvider = scope.ServiceProvider;

var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

var data = await GetAndParseDataAsync(httpClient);

var apiService = serviceProvider.GetRequiredService<IAiDevsApiService>();
var answer = new TaskAnswer<string[]>()
{
    Task = "POLIGON",
    Answer = data
};

var apiResponse = await apiService.VerifyTaskAnswerAsync(answer);
Console.WriteLine(JsonSerializer.Serialize(apiResponse));

Debug.Assert(apiResponse.IsSuccess);

static async Task<string[]> GetAndParseDataAsync(HttpClient httpClient)
{
    const string url = "https://poligon.aidevs.pl/dane.txt";

    var response = await httpClient.GetStringAsync(url);

    return response.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}
