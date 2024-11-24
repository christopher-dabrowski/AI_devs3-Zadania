using Common.AiDevsApi.Extensions;
using Common.Cache.Extensions;
using Common.FirecrawlService.Extensions;
using Common.OpenAI;
using S02E05.Models;

namespace S02E05;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS02E05(this IServiceCollection services)
    {
        services.AddOptions<S02E05Options>()
            .BindConfiguration(S02E05Options.SectionName)
            .ValidateDataAnnotations();

        services
            .AddHttpClient()
            .AddAiDevsApi()
            .AddFileCacheService()
            .AddFirecrawl()
            .AddOpenAIChatClient()
            .AddOpenAIClient();

        return services;
    }
}
