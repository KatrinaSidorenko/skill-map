using LearningPlatform.Shared.Caching.Abstractions;
using LearningPlatform.Shared.Caching.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Shared.Caching;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();
        return services;
    }
}
