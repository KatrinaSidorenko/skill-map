using SkillMap.Api.PersonalRoadmaps.CreatePersonalRoadmap;
using SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmap;
using SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmaps;

namespace SkillMap.Api.PersonalRoadmaps;

internal static class PersonalRoadmapsEndpoints
{
    internal static void MapPersonalRoadmaps(this WebApplication app)
    {
        app.MapCreatePersonalRoadmap();
        app.MapGetPersonalRoadmapSummary();
        app.MapGetPersonalRoadmaps();
    }
}
