using Core.Interfaces.LLM.Cache;

namespace Infrastructure.Services.LLM.Cache
{
    public class ChatMetadataCache : IChatMetadataCache
    {
        // In-memory placeholder until Redis is ready
        // TODO: Add Redis
        private readonly Dictionary<int, string> _cache = [];

        public Task<string?> GetValueAsync(int key)
        {
            _cache.TryGetValue(key, out var value);
            return Task.FromResult(value);
        }

        public Task SetValueAsync(int key, string value)
        {
            _cache[key] = value;
            return Task.CompletedTask;
        }
    }
}

