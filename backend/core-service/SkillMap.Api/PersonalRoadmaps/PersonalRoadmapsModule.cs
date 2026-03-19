using System.Reflection;

using SkillMap.Api.Roadmap;
using SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.AddLearningItem;
using SkillMap.Infrastructure.PersonalRoadmaps;
using SkillMap.Shared;

namespace SkillMap.Api.PersonalRoadmaps;

public static class PersonalRoadmapsModule
{
    private static Assembly CurrentModule => typeof(AddLearningItemRequest).Assembly;
    public static void RegisterPersonalRoadmaps(this WebApplication app, string module)
    {
        //if (!app.Configuration.IsModuleEnabled(module))
        //{
        //    return;
        //}

        app.UsePersonalRoadmaps();
        app.MapPersonalRoadmaps();
    }

    public static IServiceCollection AddPersonalRoadmaps(this IServiceCollection services,
        string module, IConfiguration configuration)
    {
        //if (!configuration.IsModuleEnabled(module))
        //{
        //    return services;
        //}

        services.AddRequestsValidations(CurrentModule);
        services.AddPersonalRoadmapsInfrastructure();

        return services;
    }

    private static IApplicationBuilder UsePersonalRoadmaps(this IApplicationBuilder applicationBuilder)
        => applicationBuilder;
}
