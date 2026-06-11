using SkillMap.Shared.Files;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateEmptyRoadmapWorkspace;

public record CreateEmptyRoadmapWorkspaceCommand(
    long UserId,
    string Title,
    string? Description,
    HardFile? ImageFile) : ICommand<long>;
