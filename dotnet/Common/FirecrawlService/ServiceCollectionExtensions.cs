namespace Common.FirecrawlService.Extensions;

using Common.FirecrawlService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFirecrawl(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FirecrawlOptions>(
            configuration.GetSection(FirecrawlOptions.SectionName));

        services.AddHttpClient<IFirecrawlService, FirecrawlService>();

        return services;
    }
}
