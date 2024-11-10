using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;

namespace Common.OpenAI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAIChatClient(this IServiceCollection services, string model = "gpt-4")
    {
        services.AddSingleton<ChatClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var apiKey = configuration["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("OpenAI API key not configured");

            return new ChatClient(model, apiKey);
        });

        return services;
    }
}
