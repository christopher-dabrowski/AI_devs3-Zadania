using System.Net.Http.Json;
using OpenAI.Chat;
using S03E03;
using S03E03.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddS03E03()
    .Configure<S03E03Options>(
        builder.Configuration.GetSection(nameof(S03E03Options)));

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

