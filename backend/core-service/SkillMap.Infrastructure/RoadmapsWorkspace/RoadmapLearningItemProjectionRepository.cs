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
        var totalCount = await _dbSet.Where(p => p.RoadmapWorkspaceId == workspaceId).CountAsync(ct);
        var completedCount = await _dbSet.Where(p => p.RoadmapWorkspaceId == workspaceId && p.Status == LearningStatus.Completed.ToStatusString()).CountAsync(ct);
        return (totalCount, completedCount);
    }
}
