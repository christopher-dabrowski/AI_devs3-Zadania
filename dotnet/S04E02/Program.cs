using System.Text;
using S04E02;
using S04E02.Models;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddS04E02Services();

var host = builder.Build();
var options = host.Services.GetRequiredService<IOptions<S04E02Options>>().Value;

await PrepareFineTuningData();

async Task PrepareFineTuningData(string fineTuningDataFileName = "fineTuningData.jsonl")
{
    const string CorrectDataFileName = "correct.txt";
    const string IncorrectDataFileName = "incorrect.txt";

    const string CorrectLabel = "1";
    const string IncorrectLabel = "0";

    await using var outputFile = File.OpenWrite(fineTuningDataFileName);
    await using var fileWriter = new StreamWriter(outputFile, Encoding.UTF8);
    var serializerOptions = new JsonSerializerOptions
    {
        WriteIndented = false,
    };

    async Task WriteExamples(string fileName, string label)
    {
        await foreach (var line in File.ReadLinesAsync($"Resources/{fileName}"))
        {
            var tuningExample = CreateFineTuningExample(line, label);
            var serialized = JsonSerializer.Serialize(tuningExample, serializerOptions);
            await fileWriter.WriteLineAsync(serialized);
        }
    }

    await WriteExamples(CorrectDataFileName, CorrectLabel);
    await WriteExamples(IncorrectDataFileName, IncorrectLabel);

    await fileWriter.FlushAsync();
}

FineTuningExample CreateFineTuningExample(string data, string label)
{
    const string SystemPrompt = "Categorize data";

    var systemMessage = new FineTuningChatMessage
    {
        Role = "system",
        Content = SystemPrompt,
    };
    var userInput = new FineTuningChatMessage
    {
        Role = "user",
        Content = data,
    };
    var assistantAnswer = new FineTuningChatMessage
    {
        Role = "assistant",
        Content = label,
    };

    return new FineTuningExample
    {
        Messages = [systemMessage, userInput, assistantAnswer],
    };
}