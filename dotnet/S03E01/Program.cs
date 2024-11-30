using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Extensions;
using Common.OpenAI;
using OpenAI.Chat;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi()
    .AddOpenAIChatClient()
    .AddOpenAIClient();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
var chatClient = sp.GetRequiredService<ChatClient>();

// Task implementation will go here

Console.WriteLine("Task completed");
