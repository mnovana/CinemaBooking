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
        
        public async Task<T> GetData<T>(string key)
        {
            var value = await _cache.GetStringAsync(key);

            if(!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public async Task RemoveData(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), cacheEntryOptions);
        }
    }
}
