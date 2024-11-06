namespace SharedLibrary.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime);
        Task RemoveDataAsync(string key);
    }
}
