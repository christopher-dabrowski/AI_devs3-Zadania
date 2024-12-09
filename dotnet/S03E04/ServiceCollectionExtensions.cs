using Common.AiDevsApi.Extensions;
using Common.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using S03E04.Models;

namespace S03E04;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS03E04(this IServiceCollection services)
    {
        services.AddOptions<S03E04Options>()
            .BindConfiguration(S03E04Options.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddAiDevsApi()
            .AddOpenAIChatClient()
            .AddHttpClient();
    }
}
