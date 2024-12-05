using Common.AiDevsApi.Extensions;
using Common.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using S03E03.Models;

namespace S03E03;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS03E03(this IServiceCollection services)
    {
        services.AddOptions<S03E03Options>()
            .BindConfiguration(S03E03Options.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddAiDevsApi()
            .AddOpenAIChatClient()
            .AddHttpClient();
    }
}
