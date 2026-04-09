using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapLearningItemStatusRepository : IRepository<RoadmapLearningItemStatus>
{
    Task<(int TotalItems, int CompletedItems)> GetWorkspaceProgressAsync(long workspaceId, CancellationToken ct);
}
