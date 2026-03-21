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
}
