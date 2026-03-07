using SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;

namespace SkillMap.Api.Roadmap;

internal static class PersonalizedRoadmapsEndpoints
{
    internal static void MapPersonalizedRoadmaps(this WebApplication app)
    {
        app.MapAddLearningItem();
    }
}
