using EasyCache.Core.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasyCache.Redis.Concrete
{
    /// <summary>
    /// This class includes implementation of Cache operation for Redis
    /// </summary>
    public class EasyCacheRedisManager : IEasyCacheService
    {
        private readonly IDistributedCache distributedCache;

        public EasyCacheRedisManager(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }
        public virtual T Get<T>(string key)
        {
            var str = distributedCache.GetString(key);
            if (str is null) return default(T);

            return JsonSerializer.Deserialize<T>(str);
        }

        public virtual async Task<T> GetAsync<T>(string key)
        {
            var str = await distributedCache.GetStringAsync(key);
            if (str is null) return default(T);

            return JsonSerializer.Deserialize<T>(str);
        }

        public virtual void Remove<T>(string key)
        {
            var data = distributedCache.Get(key);
            if (data != null)
                distributedCache.Remove(key);
        }

        public virtual async Task RemoveAsync<T>(string key)
        {
            var str = distributedCache.GetAsync(key);
            if (str != null) await distributedCache.RemoveAsync(key);
        }

        public virtual void Set<T>(string key, T value, TimeSpan expireTime)
        {
            var str = JsonSerializer.Serialize<T>(value);
            var option = new DistributedCacheEntryOptions().SetSlidingExpiration(expireTime);
            distributedCache.SetString(key, str, option);
        }

        public void Set<T>(string key, T value)
        {
            var str = JsonSerializer.Serialize<T>(value);
            distributedCache.SetString(key, str);
        }

        public virtual async Task SetAsync<T>(string key, T value, TimeSpan expireTime)
        {
            var str = JsonSerializer.Serialize<T>(value);
            var option = new DistributedCacheEntryOptions().SetSlidingExpiration(expireTime);
            await distributedCache.SetStringAsync(key, str, option);
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var str = JsonSerializer.Serialize<T>(value);
            await distributedCache.SetStringAsync(key, str);
        }
    }
}
