using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;

namespace Common.OpenAI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAIChatClient(this IServiceCollection services, string model = "gpt-4o") => services.AddSingleton<ChatClient>(sp =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API key not configured");

        return new ChatClient(model, apiKey);
    });

    public static IServiceCollection AddOpenAIClient(this IServiceCollection services) => services.AddSingleton<OpenAIClient>(sp =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API key not configured");

        return new OpenAIClient(apiKey);
    });
}
