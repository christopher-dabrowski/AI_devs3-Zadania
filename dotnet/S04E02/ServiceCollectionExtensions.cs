using Common.AiDevsApi.Extensions;
using S04E02.Models;

namespace S04E02;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS04E02Services(this IServiceCollection services)
    {
        services.AddOptions<S04E02Options>()
                    .BindConfiguration(S04E02Options.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

        return services
            .AddAiDevsApi();
    }
}
