using SkillMap.Core.Roadmaps;

namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmap;
public record PersonalRoadmapSummaryDto(
    string Id,
    string WorkspaceId,
    bool IsPublic,
    string Title,
    string Description,
    string ImageUrl)
{
    public static PersonalRoadmapSummaryDto Create(PersonalRoadmap dto)
        => new(dto.Id.ToString(), dto.RoadmapWorkspace.Id.ToString(), dto.IsPublic, dto.Title, dto.Description, dto.ImageUrl);
}