namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;
public record BuildAuthorWorkspaceSnapshotCommand(long WorkspaceId, string RoadmapId) : ICommand<long>;
