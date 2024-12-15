using Common;
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

var initialPeopleAndCities = await ExtractNamesAndCitiesAsync();
Console.WriteLine(JsonSerializer.Serialize(initialPeopleAndCities));

var notCheckedPeople = new HashSet<string>(initialPeopleAndCities.Names
    .Select(s => s.ToUpper())
    .Select(ReplacePolishCharacters));
var notCheckedCities = new HashSet<string>(initialPeopleAndCities.Cities
    .Select(s => s.ToUpper())
    .Select(ReplacePolishCharacters));

var checkedPeopleResults = new DefaultDictionary<string, HashSet<string>>();
var checkedCitiesResults = new DefaultDictionary<string, HashSet<string>>();

var barbaraFound = false;
const int maxAttempts = 200;
for (var i = 0; i < maxAttempts && !barbaraFound; i++)
{
    if (notCheckedPeople.Any())
    {
        var personToCheck = notCheckedPeople.First();
        var seenAt = await GetPlacesWhereWasSeen(personToCheck);
        var newCities = await AddPersonCheckResult(personToCheck, seenAt);
        
        notCheckedCities.UnionWith(newCities);
        notCheckedPeople.Remove(personToCheck);
    }
    else if (notCheckedCities.Any())
    {
        var cityToCheck = notCheckedCities.First();
        var peopleThere = await GetWhoWasThere(cityToCheck);
        var newPeople = await AddCityCheckResult(cityToCheck, peopleThere);
        
        notCheckedPeople.UnionWith(newPeople);
        notCheckedCities.Remove(cityToCheck);

        var barbaraInResponse = peopleThere
            .Any(person => person.Equals("BARBARA", StringComparison.OrdinalIgnoreCase));
        if (barbaraInResponse)
            barbaraFound = await CheckBarbaraLocationWithApi(cityToCheck);
        if (barbaraFound)
            Console.WriteLine($"Found Barbara in {cityToCheck}");
    }
    else
    {
        Console.WriteLine("No people or places left to check :(");
        break;
    }
}

Console.WriteLine($"Max tries of {maxAttempts} reached");

async Task<bool> CheckBarbaraLocationWithApi(string city)
{
    var taskAnswer = new TaskAnswer
    {
        Task = "loop",
        Answer = city
    };
    var apiResponse = await aiDevsApiService.VerifyTaskAnswerAsync(taskAnswer);
    
    Console.WriteLine(JsonSerializer.Serialize(apiResponse));
    return apiResponse.IsSuccess;
}

async Task<IReadOnlyList<string>> AddPersonCheckResult(
    string name,
    IReadOnlyList<string> foundCities)
{
    var citiesOfPerson = checkedPeopleResults[name];
    citiesOfPerson.UnionWith(foundCities);

    await File.WriteAllTextAsync("peopleToCities.json", JsonSerializer.Serialize(checkedPeopleResults));

    var newCities = foundCities
        .Where(c => !checkedCitiesResults.ContainsKey(c))
        .ToImmutableArray();
    return newCities;
}

async Task<IReadOnlyList<string>> AddCityCheckResult(
    string city,
    IReadOnlyList<string> foundPeople)
{
    var peopleInCity = checkedCitiesResults[city];
    peopleInCity.UnionWith(foundPeople);

    await File.WriteAllTextAsync("citiesToPeople.json", JsonSerializer.Serialize(checkedCitiesResults));

    var newPeople = foundPeople
        .Where(p => !checkedPeopleResults.ContainsKey(p))
        .ToImmutableArray();
    return newPeople;
}

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

async Task<ExtractNamesAndCitiesResponse> ExtractNamesAndCitiesAsync()
{
    var chatClient = sp.GetRequiredService<ChatClient>();

    var messageFilePath = "Resources/Note.txt";
    var message = await File.ReadAllTextAsync(messageFilePath);

    ChatMessage[] messages = [
        new SystemChatMessage(Prompts.ExtractNamesAndCitiesSystemPrompt),
        new UserChatMessage(message)
    ];

    var options = new ChatCompletionOptions
    {
        Temperature = 0,
    };

    var response = await chatClient.CompleteChatAsync(messages, options);
    var completion = response.Value.Content[0].Text;

    return JsonSerializer.Deserialize<ExtractNamesAndCitiesResponse>(completion)
        ?? throw new InvalidOperationException("Unable to parse the chat response");
}
