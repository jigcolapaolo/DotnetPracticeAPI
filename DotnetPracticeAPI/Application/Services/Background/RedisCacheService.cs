using Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System.Text.Json;
using IDatabase = StackExchange.Redis.IDatabase;

namespace Application.Services.Background
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDb;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _redisDb = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _redisDb.StringGetAsync(key);
                if (value.IsNullOrEmpty) return default;

                _logger.LogInformation($"Cache hit for key: {key}");
                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading from cache");
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _redisDb.StringSetAsync(
                    key,
                    JsonSerializer.Serialize(value),
                    expiry ?? TimeSpan.FromMinutes(5)
            );
        }

        public async Task RemoveAsync(string key)
        {
            await _redisDb.KeyDeleteAsync(key);
        }
    }
}
