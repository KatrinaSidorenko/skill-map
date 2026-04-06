using System.Reflection;

using SkillMap.Api.PersonalRoadmaps;
using SkillMap.Api.RoadmapAssessments;
using SkillMap.Api.RoadmapBlueprints;
using SkillMap.Api.Roadmaps;

namespace SkillMap.Api;

internal static class ModulesRegistry
{
    internal static void AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRoadmapsWorkspace(Module.RoadmapsWorkspace, configuration);
        services.AddPersonalRoadmaps(Module.PersonalRoadmaps, configuration);
        services.AddRoadmapBlueprints(Module.RoadmapBlueprints, configuration);
        services.AddRoadmapAssessments(Module.RoadmapAssessments, configuration);
    }

    internal static void RegisterModules(this WebApplication app)
    {
        app.RegisterPersonalizedRoadmaps(Module.RoadmapsWorkspace);
        app.RegisterPersonalRoadmaps(Module.PersonalRoadmaps);
        app.RegisterRoadmapBlueprints(Module.RoadmapBlueprints);
        app.RegisterRoadmapAssessments(Module.RoadmapAssessments);
    }
}