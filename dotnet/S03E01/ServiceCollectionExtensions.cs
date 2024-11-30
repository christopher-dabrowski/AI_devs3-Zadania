using Common.AiDevsApi.Extensions;
using Common.OpenAI;

namespace S03E01;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS03E01(this IServiceCollection services)
    {
        services
            .AddAiDevsApi()
            .AddOpenAIChatClient()
            .AddOpenAIClient();

        return services;
    }
}
