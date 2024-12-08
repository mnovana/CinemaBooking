using Microsoft.Extensions.Logging;
using SharedLibrary.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace SharedLibrary.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }
        
        public async Task<T?> GetDataAsync<T>(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            RedisValue? value;
            try
            {
                value = await db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Redis failed to get data: {msg}", ex.Message);
                value = null;
            }

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
            RedisValue[]? redisValues = null;

            try
            {
                redisValues = await db.StringGetAsync(redisKeys);
            }

            catch(Exception ex)
            {
                _logger.LogWarning("Redis failed to get multiple data: {msg}", ex.Message);
                return keys.Select(k => default(T?));   // return a list of nulls
            }

            var values = redisValues
                .Select(redisValue =>
                {
                    if(redisValue.IsNullOrEmpty)
                    {
                        return default;
                    }
                    else
                    {
                        return JsonSerializer.Deserialize<T>(redisValue);
                    }
                });

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

            try
            {
                await db.StringSetAsync(key, JsonSerializer.Serialize(value), expirationTime);
            }
            catch(Exception ex)
            {
                _logger.LogWarning("Redis failed to set data: {msg}", ex.Message);
            }
        }

        public async Task SetMultipleDataAsync<T>(Dictionary<string, T> entries, TimeSpan expirationTime)
        {
            var db = _connectionMultiplexer.GetDatabase();

            try
            {
                var tasks = entries.Select(entry => db.StringSetAsync(entry.Key, JsonSerializer.Serialize(entry.Value), expirationTime));

                await Task.WhenAll(tasks);
            }
            catch(Exception ex)
            {
                _logger.LogWarning("Redis failed to set multiple data: {msg}", ex.Message);
            }
        }
    }
}
