using Microsoft.Extensions.DependencyInjection;

using SkillMap.Infrastructure.EventBus.InMemory;

namespace SkillMap.Infrastructure.EventBus;
internal static class EventBusModule
{
    internal static IServiceCollection AddEventBus(this IServiceCollection services) =>
        services.AddInMemoryEventBus();
}

