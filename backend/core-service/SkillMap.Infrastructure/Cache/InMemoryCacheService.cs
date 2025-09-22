using Microsoft.Extensions.Caching.Memory;

namespace SkillMap.Infrastructure.Cache;

public class InMemoryCacheService(IMemoryCache cache) : ICacheService
{
    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(5)
        };

        cache.Set(key, value!, options);

        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        var success = cache.TryGetValue(key, out T? value);
        return Task.FromResult(success ? value : default);
    }

    public Task<(bool found, T? value)> TryGetAsync<T>(string key)
    {
        if (cache.TryGetValue(key, out var cached) && cached is T casted)
        {
            return Task.FromResult((true, casted));
        }

        return Task.FromResult((false, default(T)));
    }

    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}

