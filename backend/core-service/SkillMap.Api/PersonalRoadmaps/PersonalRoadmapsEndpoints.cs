using SkillMap.Api.PersonalRoadmaps.CreatePersonalRoadmap;

namespace SkillMap.Api.PersonalRoadmaps;

internal static class PersonalRoadmapsEndpoints
{
    internal static void MapPersonalRoadmaps(this WebApplication app)
    {
        app.MapCreatePersonalRoadmap();
    }
}
