using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;

public record GetRoadmapWorkspacesQuery(long UserId, FilteringParams FilteringParams) : ICommand<PaginationResult<RoadmapWorkspaceSummaryDto>>;