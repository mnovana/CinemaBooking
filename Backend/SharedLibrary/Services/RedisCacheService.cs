using SharedLibrary.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace SharedLibrary.Services
{
    public class RedisCacheService : ICacheService
    {
        public readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }
        
        public async Task<T?> GetDataAsync<T>(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var value = await db.StringGetAsync(key);

            if(!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public async Task<IEnumerable<T?>> GetMultipleDataAsync<T>(string[] keys)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var redisKeys = keys.Select(key => new RedisKey(key)).ToArray();
            var redisValues = await db.StringGetAsync(redisKeys);

            var values = redisValues.Select(redisValue => JsonSerializer.Deserialize<T>(redisValue));

            return values;
        }

        public async Task RemoveDataAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async Task SetDataAsync<T>(string key, T value, TimeSpan expirationTime)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.StringSetAsync(key, JsonSerializer.Serialize(value), expirationTime);
        }

        public async Task SetMultipleDataAsync<T>(Dictionary<string, T> entries, TimeSpan expirationTime)
        {
            var db = _connectionMultiplexer.GetDatabase();

            var tasks = entries.Select(entry => db.StringSetAsync(entry.Key, JsonSerializer.Serialize(entry.Value), expirationTime));

            await Task.WhenAll(tasks);
        }
    }
}
