namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateEmptyRoadmapWorkspace;

public record CreateEmptyRoadmapWorkspaceCommand(
    long UserId,
    string Title,
    string? Description,
    string? ImageUrl) : ICommand<long>;
