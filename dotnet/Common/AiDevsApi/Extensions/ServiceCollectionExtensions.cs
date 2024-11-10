using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Models;
using Common.AiDevsApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.AiDevsApi.Extensions;

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
}
