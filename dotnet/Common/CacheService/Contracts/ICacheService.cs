namespace Common.Cache.Contracts;

public interface ICacheService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string data);
    Task<bool> ExistsAsync(string key);

    Task<byte[]?> GetAsyncBytes(string key);
    Task SetAsyncBytes(string key, byte[] data);
}
