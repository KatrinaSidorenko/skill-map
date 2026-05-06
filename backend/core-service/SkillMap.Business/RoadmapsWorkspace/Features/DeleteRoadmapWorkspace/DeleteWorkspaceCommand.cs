namespace SkillMap.Business.RoadmapsWorkspace.Features.DeleteRoadmapFork;
public record DeleteWorkspaceCommand(long WorkspaceId, bool IsSoftDelete = true) : ICommand { }