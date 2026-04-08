using LearningPlatform.Shared.Api;

namespace SkillMap.Api.Roadmaps;

internal static class RoadmapsWorkspaceApiPaths
{
    private const string RoadmapsWorkspaceRootApi = $"{ApiPaths.Root}/roadmaps-workspace";

    public const string GetRoadmapWorkspaces = $"{RoadmapsWorkspaceRootApi}";
    public const string AddLearningItem = $"{RoadmapsWorkspaceRootApi}/create-item/{{userRoadmapId}}";
    public const string UpdateLearningItem = $"{RoadmapsWorkspaceRootApi}/update-item/{{userRoadmapId}}";
    public const string DeleteLearningItem = $"{RoadmapsWorkspaceRootApi}/delete-item/{{userRoadmapId}}";
    public const string CreateLearningItemConnection = $"{RoadmapsWorkspaceRootApi}/create-connection/{{userRoadmapId}}";
    public const string DeleteLearningItemConnection = $"{RoadmapsWorkspaceRootApi}/delete-connection/{{userRoadmapId}}";
    public const string GetRoadmapWorkspace = $"{RoadmapsWorkspaceRootApi}/{{userRoadmapId}}";
    public const string GetRoadmapWorkspaceSummary = $"{RoadmapsWorkspaceRootApi}/{{userRoadmapId}}/summary";
    public const string DeleteRoadmapWorkspace = $"{RoadmapsWorkspaceRootApi}/{{userRoadmapId}}";
    public const string CreateRoadmapWorkspace = $"{RoadmapsWorkspaceRootApi}";
    public const string GetWorkspaceEventsStatus = $"{RoadmapsWorkspaceRootApi}/{{userRoadmapId}}/events/status";
}