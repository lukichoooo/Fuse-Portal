using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM.Cache;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Services.LLM.Cache
{
    public class ChatMetadataCache : IChatMetadataCache
    {
        private readonly IDatabase _db;
        private readonly RedisSettings _settings;

        public ChatMetadataCache(
            IConnectionMultiplexer redis,
            IOptions<RedisSettings> options)
        {
            _db = redis.GetDatabase();
            _settings = options.Value;

            if (_settings.DefaultMinutes <= 0)
                throw new InvalidOperationException("Redis TTL must be > 0");
        }

        public async Task<string?> GetValueAsync(int key)
            => await _db.StringGetAsync(key.ToString());

        public async Task SetValueAsync(int key, string value)
            => await _db.StringSetAsync(
                    key.ToString(),
                    value,
                    TimeSpan.FromMinutes(_settings.DefaultMinutes));
    }
}

