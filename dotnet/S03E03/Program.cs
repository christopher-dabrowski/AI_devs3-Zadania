using System.Net.Http.Json;
using OpenAI.Chat;
using S03E03;
using S03E03.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddS03E03();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var showTablesTool = ChatTool.CreateFunctionTool(
    functionName: nameof(ShowTables),
    functionDescription: "List tables in the database"
);

var getTableSchemaTool = ChatTool.CreateFunctionTool(
    functionName: nameof(GetTableSchema),
    functionDescription: "Get the SQL code that was used to crate the table. You can use this to learn the table schema",
    functionParameters: BinaryData.FromBytes("""
        {
            "type": "object",
            "properties": {
                "tableName": {
                    "type": "string",
                    "description": "Name of the database table"
                }
            },
            "required": ["tableName"],
            "additionalProperties": false
        }
        """u8.ToArray())
);

var runSqlTool = ChatTool.CreateFunctionTool(
    functionName: nameof(RunDbCommand),
    functionDescription: "Run a SQL command on the database",
    functionParameters: BinaryData.FromBytes("""
        {
            "type": "object",
            "properties": {
                "command": {
                    "type": "string",
                    "description": "Text of the SQL command to run"
                }
            },
            "required": ["command"],
            "additionalProperties": false
        }
        """u8.ToArray())
);

List<ChatMessage> messages =
[
    new SystemChatMessage(
        """
        You are a helpful assistant with access to tools for interacting with a SQL database.
        Use the tools to get the correct data from the database.
        First learn of the database structure, then query it for the relevant data.
        
        Whey you are ready to give the final response start it with "ANSWER:" and provide only the requred data separated by ", "
        """),
    new UserChatMessage("które aktywne datacenter (DC_ID) są zarządzane przez pracowników, którzy są na urlopie (is_active=0)"),
];

ChatCompletionOptions options = new()
{
    Tools = { showTablesTool, getTableSchemaTool, runSqlTool },
};

var client = sp.GetRequiredService<ChatClient>();

bool requiresAction;

do
{
    requiresAction = false;
    ChatCompletion completion = await client.CompleteChatAsync(messages, options);

    switch (completion.FinishReason)
    {
        case ChatFinishReason.Stop:
            {
                messages.Add(new AssistantChatMessage(completion));
                break;
            }
        case ChatFinishReason.ToolCalls:
            {
                messages.Add(new AssistantChatMessage(completion));

                foreach (ChatToolCall toolCall in completion.ToolCalls)
                {
                    switch (toolCall.FunctionName)
                    {
                        case nameof(ShowTables):
                            {
                                string toolResult = await ShowTables();
                                messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                break;
                            }

                        case nameof(GetTableSchema):
                            {
                                using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                bool hasTableName = argumentsJson.RootElement.TryGetProperty("tableName", out JsonElement tableName);

                                if (!hasTableName)
                                    throw new ArgumentNullException(nameof(tableName), "The tableName is required.");

                                string toolResult = await GetTableSchema(tableName.GetString() ?? string.Empty);
                                messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                break;
                            }

                        case nameof(RunDbCommand):
                            {
                                using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                bool hasCommand = argumentsJson.RootElement.TryGetProperty("command", out JsonElement command);

                                if (!hasCommand)
                                    throw new ArgumentNullException(nameof(command), "The command is required.");

                                string toolResult = await RunDbCommand(command.GetString() ?? string.Empty);
                                messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                break;
                            }

                        default:
                            {
                                // Handle other unexpected calls.
                                throw new NotImplementedException();
                            }
                    }
                }

                requiresAction = true;
                break;
            }

        case ChatFinishReason.Length:
            throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

        case ChatFinishReason.ContentFilter:
            throw new NotImplementedException("Omitted content due to a content filter flag.");

        case ChatFinishReason.FunctionCall:
            throw new NotImplementedException("Deprecated in favor of tool calls.");

        default:
            throw new NotImplementedException(completion.FinishReason.ToString());
    }
} while (requiresAction);

var finalResponse = messages.Last().Content;
var reponseText = finalResponse[0].Text;
Console.WriteLine(JsonSerializer.Serialize(finalResponse));

var agentAnswerIndex = reponseText.IndexOf("ANSWER:", StringComparison.Ordinal);
var rawAnswer = agentAnswerIndex != -1 
    ? reponseText[(agentAnswerIndex + "ANSWER:".Length)..] 
    : reponseText;

var ids = rawAnswer
    .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(int.Parse)
    .ToImmutableArray();

var taskResponse = new TaskAnswer<IList<int>>()
{
    Task = "database",
    Answer = ids
};
var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();
var taskResponseResult = await aiDevsApiService.VerifyTaskAnswerAsync(taskResponse);
Console.WriteLine(JsonSerializer.Serialize(taskResponseResult));

async Task<string> ShowTables() => await RunDbCommand("show tables");

async Task<string> GetTableSchema(string tableName) => await RunDbCommand($"show create table {tableName}");

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
