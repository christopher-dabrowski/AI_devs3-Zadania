using System.Net.Http.Json;
using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Extensions;
using Common.AiDevsApi.Models;
using Common.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi()
    .AddOpenAIClient()
    .AddOpenAIChatClient();



var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var witnessStatement = await GetRobotDescription();
Console.WriteLine("Robot description:");
Console.WriteLine(witnessStatement);

var robotAppearance = await ExtractRobotAppearance(witnessStatement);
Console.WriteLine("\nRobot appearance:");
Console.WriteLine(robotAppearance);

var robotImage = await GenerateImage(robotAppearance);
Console.WriteLine("\nGenerated image URL:");
Console.WriteLine(robotImage.ImageUri);

var aiDevsApi = sp.GetRequiredService<IAiDevsApiService>();
var taskAnswer = new TaskAnswer
{
    Task = "robotid",
    Answer = robotImage.ImageUri.ToString()
};
var result = await aiDevsApi.VerifyTaskAnswerAsync(taskAnswer);
Console.WriteLine(JsonSerializer.Serialize(result));


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

async Task<GeneratedImage> GenerateImage(string description)
{
    var chatClient = sp.GetRequiredService<ChatClient>();

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage(Prompts.ConvertToImageGenerationPrompt),
        new UserChatMessage(description)
    };
    var dallePromptCompletion = await chatClient.CompleteChatAsync(messages);
    var dallePrompt = dallePromptCompletion.Value.Content[0].Text;
    Console.WriteLine("\nDALL-E prompt:");
    Console.WriteLine(dallePrompt);

    var imageClient = sp.GetRequiredService<OpenAIClient>()
        .GetImageClient("dall-e-3");

    ImageGenerationOptions options = new()
    {
        Quality = GeneratedImageQuality.Standard,
        Size = GeneratedImageSize.W1024xH1024,
        Style = GeneratedImageStyle.Natural,
        ResponseFormat = GeneratedImageFormat.Uri
    };

    var imageResult = await imageClient.GenerateImageAsync(dallePrompt, options);
    return imageResult.Value;
}
