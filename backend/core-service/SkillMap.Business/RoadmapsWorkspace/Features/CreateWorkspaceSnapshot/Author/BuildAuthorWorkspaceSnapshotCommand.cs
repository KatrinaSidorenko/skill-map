namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot.Author;
public record BuildAuthorWorkspaceSnapshotCommand(long WorkspaceId, string RoadmapId) : ICommand<long>;
