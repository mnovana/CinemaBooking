using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using SharedLibrary.Services.Interfaces;
using System.Text.Json;

namespace SharedLibrary.Services
{
    public class CacheService : ICacheService
    {
        public readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        
        public async Task<T?> GetDataAsync<T>(string key)
        {
            var value = await _cache.GetStringAsync(key);

            if(!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public async Task<IEnumerable<T>> GetMultipleDataAsync<T>(string[] keys)
        {
            var values = new List<T>();

            foreach (var key in keys)
            {
                var value = await _cache.GetStringAsync(key);

                if (!string.IsNullOrEmpty(value))
                {
                    values.Add(JsonSerializer.Deserialize<T>(value));
                }
            }

            return values;
        }

        public async Task RemoveDataAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), cacheEntryOptions);
        }

        public async Task SetMultipleDataAsync<T>(Dictionary<string, T> entries, DateTimeOffset expirationTime)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            };

            foreach(KeyValuePair<string, T> entry in entries)
            {
                await _cache.SetStringAsync(entry.Key, JsonSerializer.Serialize(entry.Value), cacheEntryOptions);
            }
        }
    }
}
