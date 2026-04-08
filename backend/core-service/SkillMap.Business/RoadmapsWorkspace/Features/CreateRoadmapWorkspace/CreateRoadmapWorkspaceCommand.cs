namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateRoadmapWorkspace;
public record CreateRoadmapWorkspaceCommand(long UserId, string RoadmapId) : ICommand<long>;