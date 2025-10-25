using LearningPlatform.Roadmap.Business.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Roadmap.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddRoadmapModule(this IServiceCollection services)
    {
        services.AddTransient<IRetriever, RoadmapRetriever>();
        services.AddScoped<IRoadmapService, RoadmapService>();
        return services;
    }
}
