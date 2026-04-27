using LearningPlatform.Roadmap.Business.Contracts;

using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Roadmap.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddRoadmapBlueprintModule(this IServiceCollection services)
    {
        services.AddTransient<IRetriever, RoadmapRetriever>();
        services.AddScoped<IRoadmapBlueprintRepository, RoadmapBlueprintRepository>();
        return services;
    }
}