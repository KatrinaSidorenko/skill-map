using System.Reflection;

using SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;
using SkillMap.Api.Roadmap;
using SkillMap.Shared;

namespace SkillMap.Api.Roadmaps;

public static class PersonalizedRoadmapsModule
{
    private static Assembly CurrentModule => typeof(AddLearningItemRequest).Assembly;
    public static void RegisterPersonalizedRoadmaps(this WebApplication app, string module)
    {
        //if (!app.Configuration.IsModuleEnabled(module))
        //{
        //    return;
        //}

        app.UsePersonalizedRoadmaps();
        app.MapPersonalizedRoadmaps();
    }

    public static IServiceCollection AddPersonalizedRoadmaps(this IServiceCollection services,
        string module, IConfiguration configuration)
    {
        //if (!configuration.IsModuleEnabled(module))
        //{
        //    return services;
        //}

        services.AddRequestsValidations(CurrentModule);
        //services.AddInfrastructure(configuration);

        return services;
    }

    private static IApplicationBuilder UsePersonalizedRoadmaps(this IApplicationBuilder applicationBuilder)
    {
        //applicationBuilder.UseInfrastructure();

        return applicationBuilder;
    }
}
