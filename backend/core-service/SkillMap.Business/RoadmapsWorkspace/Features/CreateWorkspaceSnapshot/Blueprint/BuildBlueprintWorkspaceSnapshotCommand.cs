namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot.Blueprint;
public record BuildBlueprintWorkspaceSnapshotCommand(long WorkspaceId, string RoadmapId) : ICommand<long>;
