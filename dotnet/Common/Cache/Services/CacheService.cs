using Common.Cache.Contracts;

namespace Common.Cache.Services;

public class CacheService : ICacheService
{
    private readonly string cacheDirectory;

    public CacheService(string baseDirectory)
    {
        cacheDirectory = Path.Combine(baseDirectory, "Resources", "Cache");
        Directory.CreateDirectory(cacheDirectory);
    }

    public async Task<string?> GetAsync(string key)
    {
        var filePath = GetCacheFilePath(key);

        if (!File.Exists(filePath))
            return null;

        return await File.ReadAllTextAsync(filePath);
    }

    public async Task SetAsync(string key, string data)
    {
        var filePath = GetCacheFilePath(key);
        await File.WriteAllTextAsync(filePath, data);
    }

    public Task<bool> ExistsAsync(string key)
    {
        var filePath = GetCacheFilePath(key);
        return Task.FromResult(File.Exists(filePath));
    }

    private string GetCacheFilePath(string key) =>
        Path.Combine(cacheDirectory, $"{key}.cache");
}
