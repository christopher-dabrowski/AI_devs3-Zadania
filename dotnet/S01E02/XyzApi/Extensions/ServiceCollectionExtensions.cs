using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using S01E02.XyzApi.Contracts;
using S01E02.XyzApi.Models;
using S01E02.XyzApi.Services;

namespace S01E02.XyzApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddXyzApi(this IServiceCollection services)
    {
        services.AddOptions<XyzApiOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(XyzApiOptions.SectionName).Bind(options))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IXyzApiService, XyzApiService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<XyzApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}
