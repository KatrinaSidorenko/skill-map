using LearningPlatform.Shared.Api;

namespace SkillMap.Api.PersonalRoadmaps;

internal static class PersonalRoadmapsApiPaths
{
    private const string PersonalRoadmapsRootApi = $"{ApiPaths.Root}/personal-roadmaps";

    public const string CreatePersonalRoadmap = $"{PersonalRoadmapsRootApi}";
    public const string GetPersonalRoadmap = $"{PersonalRoadmapsRootApi}/{{personalRoadmapId}}";
}
