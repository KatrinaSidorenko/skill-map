namespace SkillMap.Business.RoadmapsWorkspace.Features.UpdateRoadmapWorkspace;

public record UpdateRoadmapWorkspaceCommand(
    long WorkspaceId,
    string? Title,
    string? Description,
    string? ImageUrl) : ICommand;
