using WcfConsumer.Application.Interfaces;

namespace WcfConsumer.Infrastructure.Decorators
{
    public static class CacheHelper
    {
        public static async Task<T> GetOrSetAsync<T>(
            ICacheService cacheService,
            string key,
            ILogger logger,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpiration = null) where T : class
        {
            var cached = await cacheService.GetAsync<T>(key);
            if (cached != null)
            {
                logger.LogInformation("Cache hit for {CacheKey}", key);
                return cached;
            }

            var result = await factory();
            await cacheService.SetAsync(key, result, absoluteExpiration ?? TimeSpan.FromHours(1));
            return result;
        }
    }
}
