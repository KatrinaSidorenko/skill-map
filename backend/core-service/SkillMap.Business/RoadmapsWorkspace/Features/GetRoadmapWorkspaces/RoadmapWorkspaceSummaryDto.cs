using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;

public record RoadmapWorkspaceSummaryDto(
    string Id,
    string Title,
    string Description,
    string ImageUrl,
    double Progress,
    DateTime SavedAt,
    LearningStatus Status)
{
    public static RoadmapWorkspaceSummaryDto Create(
        long workspaceId, 
        string Title,
        string Description,
        string ImageUrl,
        DateTime savedAt,
        LearningStatus? status, 
        double? progress)
    {
        return new RoadmapWorkspaceSummaryDto(
            workspaceId.ToString(),
            Title,
            Description,
            ImageUrl,
            progress ?? 0.0,
            savedAt,
            status ?? LearningStatus.NotStarted);
    }
}
