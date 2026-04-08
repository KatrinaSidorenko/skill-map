using System.Reflection;

using SkillMap.Infrastructure.RoadmapBlueprints;
using SkillMap.Shared;

namespace SkillMap.Api.RoadmapBlueprints;

public static class RoadmapBlueprintsModule
{
    private static Assembly CurrentModule => typeof(RoadmapBlueprintsModule).Assembly;

    public static void RegisterRoadmapBlueprints(this WebApplication app, string module)
    {
        //if (!app.Configuration.IsModuleEnabled(module))
        //{
        //    return;
        //}

        app.UseRoadmapBlueprints();
        app.MapRoadmapBlueprints();
    }

    public static IServiceCollection AddRoadmapBlueprints(this IServiceCollection services,
        string module, IConfiguration configuration)
    {
        //if (!configuration.IsModuleEnabled(module))
        //{
        // return services;
        //}

        services.AddRequestsValidations(CurrentModule);
        services.AddRoadmapBlueprintsInfrastructure();

        return services;
    }

    private static IApplicationBuilder UseRoadmapBlueprints(this IApplicationBuilder applicationBuilder)
  => applicationBuilder;
}