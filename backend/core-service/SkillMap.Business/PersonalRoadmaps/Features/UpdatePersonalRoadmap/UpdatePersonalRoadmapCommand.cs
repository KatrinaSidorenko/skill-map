namespace SkillMap.Business.PersonalRoadmaps.Features.UpdatePersonalRoadmap;

public record UpdatePersonalRoadmapCommand(
    long PersonalRoadmapId,
    long UserId,
    string? Title,
    string? Description,
    string? ImageUrl) : ICommand;
