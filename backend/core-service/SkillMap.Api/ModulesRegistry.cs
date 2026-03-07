using System.Reflection;

using SkillMap.Api.Roadmaps;

namespace SkillMap.Api;

internal static class ModulesRegistry
{
    internal static void AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersonalizedRoadmaps(Module.PersonalizedRoadmaps, configuration);
    }

    internal static void RegisterModules(this WebApplication app)
    {
        app.RegisterPersonalizedRoadmaps(Module.PersonalizedRoadmaps);
    }
}
