using Microsoft.EntityFrameworkCore;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Persistence;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class RoadmapWorkspaceRepository : Repository<RoadmapWorkspace>, IRoadmapWorkspaceRepository
{
    public RoadmapWorkspaceRepository(SkillMapDbContext context) : base(context) { }

    public async Task<List<RoadmapWorkspace>> GetUserBlueprintWorkspaces(long userId, CancellationToken ct)
        => await _dbSet.AsNoTracking().Where(w => w.AuthorId == userId && w.IsActive && !w.IsInAuthorMode && w.RoadmapId != null).ToListAsync(ct);
    public async Task<bool> IsWorkspaceActive(string roadmapId, long userId, CancellationToken ct)
        => await _dbSet.AsNoTracking().AnyAsync(w => w.RoadmapId == roadmapId && w.AuthorId == userId && w.IsActive && !w.IsInAuthorMode, ct);
    public async Task<List<RoadmapWorkspace>> GetUserActiveNonAuthoredWorkspacesWithSnapshots(long userId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var skip = (pageNumber - 1) * pageSize;
        return await _dbSet.AsNoTracking()
            .Where(w => w.AuthorId == userId && w.IsActive && !w.IsInAuthorMode && w.Snapshots.Count() > 0)
            .Include(w => w.LearningItemProjections.Where(li => li.IsAvailable))
            .Skip(skip).Take(pageSize).ToListAsync(ct);
    }

    public async Task<RoadmapWorkspace?> GetUserActiveWorkspace(long workspaceId, CancellationToken ct)
        => await _dbSet.AsNoTracking()
            .Where(w => w.Id == workspaceId && w.IsActive && !w.IsInAuthorMode)
            .Include(w => w.LearningItemProjections.Where(li => li.IsAvailable))
            .FirstOrDefaultAsync(ct);
}