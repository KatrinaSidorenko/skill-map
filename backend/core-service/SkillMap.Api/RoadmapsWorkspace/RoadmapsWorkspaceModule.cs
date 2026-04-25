using System.Reflection;

using SkillMap.Api.Roadmap;
using SkillMap.Api.RoadmapsWorkspace.CreateRoadmapWorkspace;
using SkillMap.Infrastructure.PersonalizedRoadmaps;
using SkillMap.Shared;

namespace SkillMap.Api.Roadmaps;

public static class RoadmapsWorkspaceModule
{
    private static Assembly CurrentModule => typeof(CreateRoadmapWorkspaceRequest).Assembly;
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
        services.AddRequestsValidations(CurrentModule);
        services.AddRoadmapsWorkspaceInfrastructure(configuration);

        return services;
    }

    private static IApplicationBuilder UseRoadmapsWorkspace(this IApplicationBuilder applicationBuilder)
        => applicationBuilder;
}