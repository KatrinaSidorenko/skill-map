using SkillMap.Core.Roadmaps;

namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmap;
public record PersonalRoadmapSummaryDto(
    string Id,
    string Title,
    string Description,
    string ImageUrl)
{
    public static PersonalRoadmapSummaryDto Create(PersonalRoadmap dto)
        => new(dto.Id.ToString(), dto.Title, dto.Description, dto.ImageUrl);
}
