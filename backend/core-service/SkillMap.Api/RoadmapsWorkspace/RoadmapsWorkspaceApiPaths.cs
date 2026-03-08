using LearningPlatform.Shared.Api;

namespace SkillMap.Api.Roadmaps;

internal static class RoadmapsWorkspaceApiPaths
{
    private const string PersonalizedRoadmapsRootApi = $"{ApiPaths.Root}/personalized-roadmaps";

    public const string AddLearningItem = $"{PersonalizedRoadmapsRootApi}/{{userRoadmapId}}";
    public const string GetRoadmapWorkspace = $"{PersonalizedRoadmapsRootApi}/{{userRoadmapId}}";

}
