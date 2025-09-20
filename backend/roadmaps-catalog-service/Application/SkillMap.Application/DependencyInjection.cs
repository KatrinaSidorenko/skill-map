using Microsoft.Extensions.DependencyInjection;
using SkillMap.Application.InPorts.Migrator;
using SkillMap.Application.InPorts.Retriever;

namespace SkillMap.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IRetriever, RoadmapRetriever>();
        services.AddTransient<IMigrator, RoadmapMigrator>();
        return services;
    }
}
