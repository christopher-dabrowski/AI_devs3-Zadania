using AiDevsApi.Models;
using AiDevsApi.Services;
using ApiTestTask.AiDevsApi.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AiDevsApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiDevsApi(this IServiceCollection services)
    {
        services.AddOptions<AiDevsApiOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(AiDevsApiOptions.SectionName).Bind(options))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IAiDevsApiService, AiDevsApiService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AiDevsApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }

    public static IServiceCollection AddAiDevsApi(
        this IServiceCollection services,
        Action<AiDevsApiOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddHttpClient<IAiDevsApiService, AiDevsApiService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AiDevsApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}