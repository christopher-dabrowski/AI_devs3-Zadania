using System.Text.Json.Nodes;
using OpenAI.Chat;
using S04E04;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddS04E04Services();
var app = builder.Build();

app.MapGet("/", () => "Hi 👋");

app.MapPost("/", async (
    FlightDescriptionRequest flightDescription,
    ChatClient chatClient,
    ILogger<Program> logger) =>
{
    logger.LogInformation(flightDescription.Instruction);

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage(Prompts.DeduceFlightDestination),
        new UserChatMessage(flightDescription.Instruction)
    };
    var options = new ChatCompletionOptions
    {
        Temperature = 0.3f
    };

    var completion = await chatClient.CompleteChatAsync(messages, options);
    var response = JsonNode.Parse(completion.Value.Content[0].Text)
        ?? throw new InvalidOperationException("The original JSON was null");

    logger.LogInformation(response.ToJsonString());
    return Results.Ok(response);
});

await app.RunAsync();

