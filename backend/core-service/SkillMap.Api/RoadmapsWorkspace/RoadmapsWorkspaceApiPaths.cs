using LearningPlatform.Shared.Api;

namespace SkillMap.Api.Roadmaps;

internal static class RoadmapsWorkspaceApiPaths
{
    private const string RoadmapsWorkspaceRootApi = $"{ApiPaths.Root}/roadmaps-workspace";

    public const string GetRoadmapWorkspaces = $"{RoadmapsWorkspaceRootApi}";
    public const string AddLearningItem = $"{RoadmapsWorkspaceRootApi}/create-item/{{userRoadmapId}}";
    public const string GetRoadmapWorkspace = $"{RoadmapsWorkspaceRootApi}/{{userRoadmapId}}";
    public const string CreateRoadmapWorkspace = $"{RoadmapsWorkspaceRootApi}";
}
