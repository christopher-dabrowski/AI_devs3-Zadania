namespace Common.Cache.Contracts;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T data);
    Task<bool> ExistsAsync(string key);

    Task<byte[]?> GetAsyncBytes(string key);
    Task SetAsyncBytes(string key, byte[] data);
}
