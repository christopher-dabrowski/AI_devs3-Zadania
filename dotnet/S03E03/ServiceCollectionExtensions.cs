using Common.AiDevsApi.Extensions;
using Common.OpenAI;

namespace S03E03;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS03E03(this IServiceCollection services)
    {

        return services
            .AddAiDevsApi()
            .AddOpenAIClient();
    }
}
