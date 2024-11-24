using Common.Cache.Contracts;

namespace Common.Cache.Services;

public class FileCacheService : ICacheService
{
    private readonly string _cacheDirectory;

    public FileCacheService(string cacheDirectory)
    {
        _cacheDirectory = cacheDirectory;
    }

    public async Task<string?> GetAsync(string key)
    {
        var filePath = GetCacheFilePath(key);

        if (!File.Exists(filePath))
            return null;

        return await File.ReadAllTextAsync(filePath);
    }

    public async Task<byte[]?> GetAsyncBytes(string key)
    {
        var filePath = GetCacheFilePath(key);

        if (!File.Exists(filePath))
            return null;

        return await File.ReadAllBytesAsync(filePath);
    }

    public async Task SetAsync(string key, string data)
    {
        var filePath = GetCacheFilePath(key);
        await File.WriteAllTextAsync(filePath, data);
    }

    public async Task SetAsyncBytes(string key, byte[] data)
    {
        var filePath = GetCacheFilePath(key);
        await File.WriteAllBytesAsync(filePath, data);
    }

    public Task<bool> ExistsAsync(string key)
    {
        var filePath = GetCacheFilePath(key);
        return Task.FromResult(File.Exists(filePath));
    }

    private string GetCacheFilePath(string key) =>
        Path.Combine(_cacheDirectory, $"{key}.cache");
}
