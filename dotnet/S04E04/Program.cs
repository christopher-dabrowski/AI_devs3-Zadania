using S04E04;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddS04E04Services();
var app = builder.Build();

app.MapGet("/", () => "Hi 👋");

app.MapPost("/", async (
    FlightDescriptionRequest flightDescription,
    ILogger<Program> logger) =>
{
    logger.LogInformation(flightDescription.Instruction);

    return Results.InternalServerError("Not implemented yet");
});

await app.RunAsync();

