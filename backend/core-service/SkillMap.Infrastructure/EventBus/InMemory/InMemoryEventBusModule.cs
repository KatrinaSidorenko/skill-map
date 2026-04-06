using Microsoft.Extensions.DependencyInjection;

using SkillMap.Shared.EventBus;

namespace SkillMap.Infrastructure.EventBus.InMemory;
public static class InMemoryEventBusModule
{
    public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services)
    {
        services.AddScoped<IEventBus, InMemoryEventBus>();

        return services;
    }
}