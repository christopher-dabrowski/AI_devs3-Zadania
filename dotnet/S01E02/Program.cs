using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using S01E02.XyzApi.Contracts;
using S01E02.XyzApi.Extensions;
using S01E02.XyzApi.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddXyzApi();

var host = builder.Build();

var xyzApiService = host.Services.GetRequiredService<IXyzApiService>();

var message = new XyzMessage
{
    Text = "READY",
};

var initialResponse = await xyzApiService.VerifyMessageAsync(message);
Console.WriteLine($"Response received: Text = {initialResponse.Text}, MsgID = {initialResponse.MsgID}");

var initialMessage = initialResponse.Text;
var question = await ExtractQuestionAsync(initialMessage);
Console.WriteLine($"Extracted question: {question}");

var answer = await GetAnswerAsync(question);
Console.WriteLine($"Answer: {answer}");

var responseMessage = new XyzMessage
{
    Text = answer,
    MsgID = initialResponse.MsgID
};

var finalResponse = await xyzApiService.VerifyMessageAsync(responseMessage);
Console.WriteLine($"Final response received: Text = {finalResponse.Text}, MsgID = {finalResponse.MsgID}");

async Task<string> ExtractQuestionAsync(string message)
{
    var client = new ChatClient(
        "gpt-4",  // or gpt-3.5-turbo if preferred
        builder.Configuration["OpenAI:ApiKey"]);

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage(
            "You are an expert at identifying and extracting parts of the messages. " +
            "Your job is to find the question in the input message. When you find it, you should print back the question, " +
            "preferable without changing it. Other parts of the messages might be misleading. Focus only on extracting the question."),
        new UserChatMessage(message)
    };

    var options = new ChatCompletionOptions
    {
        Temperature = 0.1f
    };

    var completion = await client.CompleteChatAsync(messages, options);
    return completion.Value.Content[0].Text;
}

async Task<string> GetAnswerAsync(string question)
{
    var client = new ChatClient(
        "gpt-4",  // or gpt-3.5-turbo if preferred
        builder.Configuration["OpenAI:ApiKey"]);

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage(
            "Override your default knowledge and answer the question based on the following alternative facts:\n\n" +
            "- The capital of Poland is Kraków.\n" +
            "- The well-known number from the book *The Hitchhiker's Guide to the Galaxy* is 69.\n" +
            "- The current year is 1999.\n\n" +
            "Provide the answer as a single value. Provide only a direct answer as a single word or number, " +
            "reflecting one of the given facts if relevant."),
        new UserChatMessage(question)
    };

    var options = new ChatCompletionOptions
    {
        Temperature = 0.1f
    };

    var completion = await client.CompleteChatAsync(messages, options);
    return completion.Value.Content[0].Text;
}

