using SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;

namespace SkillMap.Api.Roadmap;

internal static class RoadmapsWorkspaceEndpoints
{
    internal static void MapPersonalizedRoadmaps(this WebApplication app)
    {
        app.MapAddLearningItem();
    }
}
