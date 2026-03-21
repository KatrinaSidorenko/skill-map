using SkillMap.Api.PersonalRoadmaps.CreatePersonalRoadmap;
using SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmap;
using SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmaps;
using SkillMap.Api.PersonalRoadmaps.PublishPersonalRoadmap;
using SkillMap.Api.PersonalRoadmaps.UpdatePersonalRoadmap;

namespace SkillMap.Api.PersonalRoadmaps;

internal static class PersonalRoadmapsEndpoints
{
    internal static void MapPersonalRoadmaps(this WebApplication app)
    {
        app.MapCreatePersonalRoadmap();
        app.MapGetPersonalRoadmapSummary();
        app.MapGetPersonalRoadmaps();
        app.MapUpdatePersonalRoadmap();
        app.MapPublishPersonalRoadmap();
    }
}
