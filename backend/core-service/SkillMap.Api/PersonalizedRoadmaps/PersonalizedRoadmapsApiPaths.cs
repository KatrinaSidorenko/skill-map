using LearningPlatform.Shared.Api;

namespace SkillMap.Api.Roadmaps;

internal static class PersonalizedRoadmapsApiPaths
{
    private const string PersonalizedRoadmapsRootApi = $"{ApiPaths.Root}/personalized-roadmaps";

    public const string AddLearningItem = $"{PersonalizedRoadmapsRootApi}/{{userRoadmapId}}";
    public const string GetPersonalizedRoadmap = $"{PersonalizedRoadmapsRootApi}/{{userRoadmapId}}";

}
