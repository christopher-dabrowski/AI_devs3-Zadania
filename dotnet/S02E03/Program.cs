using System.Net.Http.Json;
using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Extensions;
using Common.AiDevsApi.Models;
using Common.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using S02E03.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi()
    .AddOpenAIChatClient();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var robotDescription = await GetRobotDescription();
Console.WriteLine("Robot description:");
Console.WriteLine(robotDescription);

var robotAppearance = await ExtractRobotAppearance(robotDescription);
Console.WriteLine("\nRobot appearance:");
Console.WriteLine(robotAppearance);

async Task<string> ExtractRobotAppearance(string description)
{
    var chatClient = sp.GetRequiredService<ChatClient>();

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage(Prompts.ExtractRobotAppearance),
        new UserChatMessage(description)
    };

    var options = new ChatCompletionOptions
    {
        Temperature = 0.3f
    };

    var completion = await chatClient.CompleteChatAsync(messages, options);
    return completion.Value.Content[0].Text;
}

async Task<string> GetRobotDescription()
{
    var client = sp.GetRequiredService<IAiDevsApiService>().HttpClient;
    var options = sp.GetRequiredService<IOptions<AiDevsApiOptions>>().Value;

    var robotDescriptionResponse = await client.GetFromJsonAsync<RobotDescriptionApiResponse>($"data/{options.ApiKey}/robotid.json");
    return robotDescriptionResponse?.Description ?? throw new InvalidOperationException("Failed to deserialize robot description");
}
