using Common.Cache.Contracts;
using Common.Cache.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Cache.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCacheService(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService>(sp =>
            new CacheService(AppContext.BaseDirectory));
        return services;
    }
}
