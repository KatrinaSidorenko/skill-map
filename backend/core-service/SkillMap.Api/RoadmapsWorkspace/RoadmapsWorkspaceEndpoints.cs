using SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;
using SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaces;

namespace SkillMap.Api.Roadmap;

internal static class RoadmapsWorkspaceEndpoints
{
    internal static void MapRoadmapsWorkspace(this WebApplication app)
    {
        app.MapGetRoadmapWorkspaces();
        app.MapAddLearningItem();
    }
}
