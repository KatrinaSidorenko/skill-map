using LearningPlatform.Shared.Caching.Abstractions;

using Microsoft.Extensions.Caching.Memory;

namespace LearningPlatform.Shared.Caching.Implementations;

public class InMemoryCacheService(IMemoryCache cache) : ICacheService
{
    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(5)
        };

        cache.Set(key, value!, options);

        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var success = cache.TryGetValue(key, out T? value);
        return Task.FromResult(success ? value : default);
    }

    public Task<(bool found, T? value)> TryGetAsync<T>(string key, CancellationToken ct = default)
    {
        if (cache.TryGetValue(key, out var cached) && cached is T casted)
        {
            return Task.FromResult((true, casted));
        }

        return Task.FromResult((false, default(T)));
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}