using LearningPlatform.Roadmap.Business.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace SkillMap.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddRoadmapModule(this IServiceCollection services)
    {
        services.AddTransient<IRetriever, RoadmapRetriever>();
        services.AddTransient<IMigrator, RoadmapMigrator>();
        return services;
    }
}
