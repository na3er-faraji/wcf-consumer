using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using WcfConsumer.Application.Interfaces;

namespace WcfConsumer.Infrastructure.Caching
{
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data)) return default;

            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (absoluteExpiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = absoluteExpiration;

            var json = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, json, options);
        }
    }
}
