namespace SkillMap.Business.RoadmapsWorkspace.Features.DeleteRoadmapFork;
public record DeleteWorkspaceCommand(string RoadmapId, long UserId) : ICommand { }
