using Common.Cache.Contracts;
using Common.Cache.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Cache.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileCacheService(
        this IServiceCollection services,
        string? customCacheDirectory = null)
    {
        var cacheDirectory = customCacheDirectory ??
            Path.Combine(AppContext.BaseDirectory, "Resources", "Cache");

        Directory.CreateDirectory(cacheDirectory);

        return services
        .AddSingleton<ICacheService>(sp => new FileCacheService(cacheDirectory));
    }
}
