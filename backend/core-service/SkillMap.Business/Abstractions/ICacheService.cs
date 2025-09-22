namespace SkillMap.Infrastructure.Cache;

public interface ICacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);
    Task<T?> GetAsync<T>(string key);
    Task<(bool found, T? value)> TryGetAsync<T>(string key);
    Task RemoveAsync(string key);
}
