using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprintSummary;

public record GetRoadmapBlueprintSummaryQuery(FilteringParams FilteringParams) : ICommand<PaginationResult<RoadmapBlueprintSummaryDto>>;
