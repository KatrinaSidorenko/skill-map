namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;
public record BuildWorkspaceSnapshotCommand(long WorkspaceId, string RoadmapId) : ICommand<long>;