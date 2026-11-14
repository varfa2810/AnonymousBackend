namespace AnonymousApplication.Interfaces
{
    public interface IRedisCache
    {
        Task<T?> GetAsync<T>(string key) where T : class;

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;

        Task<bool> RemoveAsync(string key);
    }
}
