using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient();
    })
    .Build();

await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

var config = sp.GetRequiredService<IConfiguration>();
var loginFormUrl = config["LoginForm"];
var openAiApiKey = config["OpenAI:ApiKey"];

var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
var website = await httpClient.GetStringAsync(loginFormUrl);

var htmlDoc = new HtmlDocument();
htmlDoc.LoadHtml(website);

var questionElement = htmlDoc.GetElementbyId("human-question");
var question = questionElement?.InnerText?.Trim() ?? throw new Exception("Question not found");

ChatClient client = new(
    model: "gpt-4", // or gpt-3.5-turbo
    apiKey: openAiApiKey
);

ChatCompletion completion = await client.CompleteChatAsync([
    new SystemChatMessage("You must respond with only a numerical value. No words or explanations - just the number."),
    new UserChatMessage(question)
]);

var answer = completion.Content[0].Text.Trim();

var formContent = new FormUrlEncodedContent(
[
    new KeyValuePair<string, string>("username", config["S01E01:Username"] ?? ""),
    new KeyValuePair<string, string>("password", config["S01E01:Password"] ?? ""),
    new KeyValuePair<string, string>("answer", answer),
]);

var response = await httpClient.PostAsync(loginFormUrl, formContent);
response.EnsureSuccessStatusCode();

var result = await response.Content.ReadAsStringAsync();
Console.WriteLine(result);
