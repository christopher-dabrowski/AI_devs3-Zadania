using Common.AiDevsApi.Extensions;
using Common.OpenAi.Extensions;
using S03E05.Models;

namespace S03E05;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS03E05(this IServiceCollection services)
    {
        services.AddOptions<S03E05Options>()
            .BindConfiguration(S03E05Options.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddAiDevsApi()
            .AddNeo4j();
    }
}
