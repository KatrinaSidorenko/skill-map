using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprintSummary;

public record GetRoadmapBlueprintSummaryQuery(FilteringParams FilteringParams, long UserId) : ICommand<PaginationResult<RoadmapBlueprintSummaryDto>>;