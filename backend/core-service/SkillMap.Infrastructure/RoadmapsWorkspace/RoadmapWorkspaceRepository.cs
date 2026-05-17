using System.Linq.Expressions;

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
    public async Task<List<RoadmapWorkspace>> GetUserActiveNonAuthoredWorkspacesWithProjections(
        long userId,
        int pageNumber,
        int pageSize,
        Expression<Func<RoadmapWorkspace, bool>>? filter = null,
        Func<IQueryable<RoadmapWorkspace>,
        IOrderedQueryable<RoadmapWorkspace>>? orderBy = null,
        CancellationToken ct = default)
    {
        var skip = (pageNumber - 1) * pageSize;
        IQueryable<RoadmapWorkspace> query = _dbSet
            .AsNoTracking()
            .Where(w => w.AuthorId == userId && w.IsActive && !w.IsInAuthorMode && w.Snapshots.Count() > 0)
            .Include(w => w.LearningItemProjections.Where(li => li.IsAvailable));

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
            query = orderBy(query);

        return await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<RoadmapWorkspace?> GetUserActiveWorkspace(long workspaceId, CancellationToken ct)
        => await _dbSet.AsNoTracking()
            .Where(w => w.Id == workspaceId && w.IsActive && !w.IsInAuthorMode)
            .Include(w => w.LearningItemProjections.Where(li => li.IsAvailable))
            .FirstOrDefaultAsync(ct);
}