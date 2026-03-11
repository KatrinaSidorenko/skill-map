using LearningPlatform.Roadmap.Business.Contracts.Models;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprintSummary;

public record RoadmapBlueprintSummaryDto(
    string Id,
    string Title,
    string Description,
    string ImageUrl)
{
    public static RoadmapBlueprintSummaryDto Create(PlainRoadmapDto dto)
        => new (dto.Id, dto.Title, dto.Description, dto.ImageUrl);
}
