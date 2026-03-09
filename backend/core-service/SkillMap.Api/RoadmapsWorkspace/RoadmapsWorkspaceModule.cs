using System.Reflection;

using SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;
using SkillMap.Api.Roadmap;
using SkillMap.Infrastructure.PersonalizedRoadmaps;
using SkillMap.Shared;

namespace SkillMap.Api.Roadmaps;

public static class RoadmapsWorkspaceModule
{
    private static Assembly CurrentModule => typeof(AddLearningItemRequest).Assembly;
    public static void RegisterPersonalizedRoadmaps(this WebApplication app, string module)
    {
        //if (!app.Configuration.IsModuleEnabled(module))
        //{
        //    return;
        //}

        app.UseRoadmapsWorkspace();
        app.MapRoadmapsWorkspace();
    }

    public static IServiceCollection AddRoadmapsWorkspace(this IServiceCollection services,
        string module, IConfiguration configuration)
    {
        //if (!configuration.IsModuleEnabled(module))
        //{
        //    return services;
        //}

        services.AddRequestsValidations(CurrentModule);
        services.AddRoadmapsWorkspaceInfrastructure();

        return services;
    }

    private static IApplicationBuilder UseRoadmapsWorkspace(this IApplicationBuilder applicationBuilder)
        => applicationBuilder;
}
