using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;

public record RoadmapWorkspaceSummaryDto(
    string Id,
    double Progress,
    DateTime SavedAt,
    LearningStatus Status)
{
    public static RoadmapWorkspaceSummaryDto Create(
        long workspaceId, 
        DateTime savedAt,
        LearningStatus? status, 
        double? progress)
    {
        return new RoadmapWorkspaceSummaryDto(
            workspaceId.ToString(),
            progress ?? 0.0,
            savedAt,
            status ?? LearningStatus.NotStarted);
    }
}
