using Microsoft.EntityFrameworkCore;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Persistence;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class RoadmapLearningItemProjectionRepository : Repository<RoadmapLearningProjection>, IRoadmapLearningItemProjectionRepository
{
    public RoadmapLearningItemProjectionRepository(SkillMapDbContext context) : base(context) { }
    
    public async Task<(int TotalItems, int CompletedItems)> GetWorkspaceProgressAsync(long workspaceId, CancellationToken ct)
    {
        var totalCount = await _dbSet.Where(p => p.RoadmapWorkspaceId == workspaceId && p.IsAvailable && p.Type == LearningItemType.SubTopic).CountAsync(ct);
        var completedCount = await _dbSet.Where(p => p.RoadmapWorkspaceId == workspaceId && p.IsAvailable && p.Type == LearningItemType.SubTopic && (p.Status == LearningStatus.Completed.ToStatusString() || p.Status == LearningStatus.Skip.ToStatusString())).CountAsync(ct);
        return (totalCount, completedCount);
    }
}
