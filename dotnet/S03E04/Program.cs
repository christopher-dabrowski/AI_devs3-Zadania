using Microsoft.Extensions.Logging;
using S03E04;
using S03E04.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddS03E04();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var logger = sp.GetRequiredService<ILogger<Program>>();
var aiDevsApiService = sp.GetRequiredService<IAiDevsApiService>();

SearchRequest CreateSearchRequest(string query)
{
    var aiDevsOptions = sp.GetRequiredService<IOptions<AiDevsApiOptions>>().Value;
    return new()
    {
        ApiKey = aiDevsOptions.ApiKey,
        Query = query,
    };
}

async Task<IReadOnlyList<string>> GetPlacesWhereWasSeen(string name)
    => await RunQuery("/people", name);

async Task<IReadOnlyList<string>> GetWhoWasThere(string town)
    => await RunQuery("/places", town);

async Task<IReadOnlyList<string>> RunQuery(string endpoint, string nameOrTown)
{
    var httpClient = aiDevsApiService.HttpClient;

    var query = ReplacePolishCharacters(nameOrTown);
    var request = CreateSearchRequest(query);
    
    logger.LogDebug("Endpoint: {endpoint} Query: {query}", endpoint, query);
    var response = await httpClient.PostAsJsonAsync(endpoint, request);

    response.EnsureSuccessStatusCode();
    var searchResult = await response.Content.ReadFromJsonAsync<SearchResult>()
        ?? throw new InvalidOperationException("API returned null");

    var resultsMessage = searchResult.Message;
    logger.LogDebug("Results: {results}", resultsMessage);

    if (resultsMessage == Consts.RestrictedDataResponse)
        return [];
    
    return resultsMessage
        .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

string ReplacePolishCharacters(string value)
{
    var polishToLatinMap = new Dictionary<char, char>
    {
        {'ą', 'a'}, {'Ą', 'A'},
        {'ć', 'c'}, {'Ć', 'C'},
        {'ę', 'e'}, {'Ę', 'E'},
        {'ł', 'l'}, {'Ł', 'L'},
        {'ń', 'n'}, {'Ń', 'N'},
        {'ó', 'o'}, {'Ó', 'O'},
        {'ś', 's'}, {'Ś', 'S'},
        {'ź', 'z'}, {'Ź', 'Z'},
        {'ż', 'z'}, {'Ż', 'Z'}
    };

    return new string(value
        .Select(c => polishToLatinMap.GetValueOrDefault(c, c))
        .ToArray()
    );
}
