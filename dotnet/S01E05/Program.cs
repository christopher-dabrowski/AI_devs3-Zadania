using System.Text;
using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Extensions;
using Common.AiDevsApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OllamaSharp.Models;
using S01E05;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;
var config = sp.GetRequiredService<IConfiguration>();

var uncensoredText = await GetUncensoredText(sp);
Console.WriteLine(uncensoredText);

var ollama = ConfigureOllamaClient(sp, config);

var censoredText = await HybridApproach(uncensoredText);
Console.WriteLine(censoredText);

var taskAnswer = new TaskAnswer
{
    Task = "CENZURA",
    Answer = censoredText
};

var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
var apiResponse = await aiDevsApiService.VerifyTaskAnswerAsync(taskAnswer);
Console.WriteLine(JsonSerializer.Serialize(apiResponse));

async Task<string> HybridApproach(string uncensoredText)
{
    var request = new GenerateRequest()
    {
        System = Prompts.IdentificationPrompt,
        Prompt = uncensoredText,
        Stream = true,
    };

    var fullResponse = new StringBuilder();
    await foreach (var stream in ollama.GenerateAsync(request))
    {
        Console.Write(stream?.Response);
        fullResponse.Append(stream?.Response);
    }
    Console.WriteLine();

    var identifiedElements = JsonSerializer.Deserialize<string[]>(fullResponse.ToString()) ?? [];

    var censoredText = new StringBuilder(uncensoredText);
    foreach (var element in identifiedElements)
        censoredText.Replace(element, "CENZURA");

    return censoredText.ToString();
}

async Task<string> FullLlmApproach(string uncensoredText)
{
    var request = new GenerateRequest()
    {
        System = Prompts.CensorshipSystem,
        Prompt = uncensoredText,
        Stream = true,
    };

    var fullResponse = new StringBuilder();
    await foreach (var stream in ollama.GenerateAsync(request))
    {
        Console.Write(stream?.Response);
        fullResponse.Append(stream?.Response);
    }
    Console.WriteLine();

    return fullResponse.ToString();
}

async Task<string> GetUncensoredText(IServiceProvider sp)
{
    var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
    var aiDevsOptions = sp.GetRequiredService<IOptions<AiDevsApiOptions>>().Value;

    return await aiDevsApiService.HttpClient.GetStringAsync($"data/{aiDevsOptions.ApiKey}/cenzura.txt");
}

OllamaApiClient ConfigureOllamaClient(IServiceProvider sp, IConfiguration config)
{
    var ollamaConfig = config.GetSection("Ollama").Get<OllamaApiClient.Configuration>()
        ?? throw new InvalidOperationException("Ollama configuration is missing");

    var ollamaHttpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("ollama");
    ollamaHttpClient.BaseAddress = ollamaConfig.Uri;
    ollamaHttpClient.Timeout = TimeSpan.FromMinutes(15);

    return new OllamaApiClient(ollamaHttpClient, ollamaConfig.Model);
}
