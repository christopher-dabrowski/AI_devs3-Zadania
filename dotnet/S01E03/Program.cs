using System.Data;
using OpenAI.Chat;
using S01E03.Models;
using Microsoft.Extensions.DependencyInjection;
using Common.OpenAI;
using Common.AiDevsApi.Extensions;
using Common.AiDevsApi.Models;
using Microsoft.Extensions.Options;
using Common.AiDevsApi.Contracts;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi()
    .AddOpenAIChatClient();

var host = builder.Build();

await using var faultyCalibrationFile = File.OpenRead("Resources/faulty-calibration-file.json");
var calibrationData = await JsonSerializer.DeserializeAsync<CalibrationData>(faultyCalibrationFile)
    ?? throw new InvalidOperationException("Failed to deserialize calibration data");

var fixedCalibrationData = await FixCalibrationDataAsync(calibrationData);

var aiDevsOptions = host.Services.GetRequiredService<IOptions<AiDevsApiOptions>>().Value;
fixedCalibrationData.ApiKey = aiDevsOptions.ApiKey;


var taskAnswer = new TaskAnswer<CalibrationData>
{
    Task = "JSON",
    Answer = fixedCalibrationData
};

var aiDevsService = host.Services.GetRequiredService<IAiDevsApiService>();
var response = await aiDevsService.VerifyTaskAnswerAsync(taskAnswer, endpoint: "/report");

Console.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

async Task<CalibrationData> FixCalibrationDataAsync(CalibrationData calibrationData)
{
    foreach (var testData in calibrationData.TestData)
    {
        testData.Answer = FixEquation(testData.Question);

        if (testData.Test is not null)
        {
            testData.Test.Answer = await AnswerQuestionAsync(testData.Test.Question);
        }
    }

    return calibrationData;
}

int FixEquation(string equation)
{
    var result = Calculator.Compute(equation, null);

    return result is int intResult
        ? intResult
        : int.Parse(result?.ToString() ?? "0");
}

async Task<string> AnswerQuestionAsync(string question)
{
    var client = host.Services.GetRequiredService<ChatClient>();

    var messages = new ChatMessage[]
    {
        new SystemChatMessage("You are a helpful assistant. Provide very brief, one-word answers when possible."),
        new UserChatMessage(question)
    };

    var completion = await client.CompleteChatAsync(messages);
    return completion.Value.Content[0].Text.Trim();
}

public partial class Program
{
    private static readonly DataTable Calculator = new();
}
