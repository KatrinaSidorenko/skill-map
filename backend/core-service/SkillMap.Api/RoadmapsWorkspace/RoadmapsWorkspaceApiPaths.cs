using LearningPlatform.Shared.Api;

namespace SkillMap.Api.Roadmaps;

internal static class RoadmapsWorkspaceApiPaths
{
    private const string RoadmapsWorkspaceRootApi = $"{ApiPaths.Root}/roadmaps-workspace";

    public const string GetRoadmapWorkspaces = $"{RoadmapsWorkspaceRootApi}";
    public const string AddLearningItem = $"{RoadmapsWorkspaceRootApi}/create-item/{{workspaceId}}";
    public const string UpdateLearningItem = $"{RoadmapsWorkspaceRootApi}/update-item/{{workspaceId}}";
    public const string DeleteLearningItem = $"{RoadmapsWorkspaceRootApi}/delete-item/{{workspaceId}}";
    public const string CreateLearningItemConnection = $"{RoadmapsWorkspaceRootApi}/create-connection/{{workspaceId}}";
    public const string DeleteLearningItemConnection = $"{RoadmapsWorkspaceRootApi}/delete-connection/{{workspaceId}}";
    public const string GetRoadmapWorkspace = $"{RoadmapsWorkspaceRootApi}/{{workspaceId}}";
    public const string GetRoadmapWorkspaceSummary = $"{RoadmapsWorkspaceRootApi}/{{workspaceId}}/summary";
    public const string CreateRoadmapWorkspace = $"{RoadmapsWorkspaceRootApi}";
}
