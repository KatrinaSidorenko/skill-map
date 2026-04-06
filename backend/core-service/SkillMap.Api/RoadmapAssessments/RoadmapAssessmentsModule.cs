using SkillMap.Infrastructure.RoadmapAssessments;

namespace SkillMap.Api.RoadmapAssessments;

public static class RoadmapAssessmentsModule
{
    public static void RegisterRoadmapAssessments(this WebApplication app, string module)
    {
        app.MapRoadmapAssessments();
    }

    public static IServiceCollection AddRoadmapAssessments(this IServiceCollection services,
         string module, IConfiguration configuration)
    {
        services.AddRoadmapAssessmentsInfrastructure();
        return services;
    }
}