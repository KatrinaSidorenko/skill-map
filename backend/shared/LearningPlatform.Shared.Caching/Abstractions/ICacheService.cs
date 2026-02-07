namespace LearningPlatform.Shared.Caching.Abstractions;

public interface ICacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken ct = default);
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task<(bool found, T? value)> TryGetAsync<T>(string key, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}