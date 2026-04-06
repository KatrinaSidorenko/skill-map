using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapTest;

namespace SkillMap.Infrastructure.RoadmapAssessments;

public static class RoadmapAssessmentMediationModule
{
    public static IServiceCollection AddRoadmapAssessmentsInfrastructure(this IServiceCollection services)
    {
        var commandHandlersAssembly = typeof(IRoadmapAssessmentModule).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(commandHandlersAssembly));

        services.AddScoped<IRoadmapAssessmentModule, RoadmapAssessmentModule>();

        services.AddRoadmapTestModule();

        return services;
    }
}