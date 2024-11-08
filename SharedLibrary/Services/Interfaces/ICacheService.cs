namespace SharedLibrary.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task<IEnumerable<T?>> GetMultipleDataAsync<T>(string[] keys);
        Task SetDataAsync<T>(string key, T value, TimeSpan expirationTime);
        Task SetMultipleDataAsync<T>(Dictionary<string, T> entries, TimeSpan expirationTime);
        Task RemoveDataAsync(string key);
    }
}
