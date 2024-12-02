namespace Common.Cache.Contracts;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<string?> GetAsync(string key);
    Task SetAsync<T>(string key, T data);
    Task SetAsync(string key, string data);
    Task<bool> ExistsAsync(string key);

    Task<byte[]?> GetAsyncBytes(string key);
    Task SetAsyncBytes(string key, byte[] data);
}
