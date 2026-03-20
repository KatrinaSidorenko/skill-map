using SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaceSummary;

public record GetRoadmapWorkspaceSummaryQuery(long WorkspaceId) : ICommand<RoadmapWorkspaceSummaryDto>;
