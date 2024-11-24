namespace Common.FirecrawlService.Extensions;

using Common.FirecrawlService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFirecrawl(this IServiceCollection services)
    {
        services.AddOptions<FirecrawlOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(FirecrawlOptions.SectionName).Bind(options))
            .ValidateDataAnnotations();

        services.AddHttpClient<IFirecrawlService, FirecrawlService>();

        return services;
    }
}
