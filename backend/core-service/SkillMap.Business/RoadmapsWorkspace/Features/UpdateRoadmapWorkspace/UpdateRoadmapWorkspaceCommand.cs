using SkillMap.Shared.Files;

namespace SkillMap.Business.RoadmapsWorkspace.Features.UpdateRoadmapWorkspace;

public record UpdateRoadmapWorkspaceCommand(
    long WorkspaceId,
    string? Title,
    string? Description,
    HardFile? ImageFile) : ICommand;
