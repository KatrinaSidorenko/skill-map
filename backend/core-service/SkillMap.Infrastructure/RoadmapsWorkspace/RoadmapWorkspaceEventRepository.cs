using System.Threading;

using Microsoft.EntityFrameworkCore;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Persistence;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class RoadmapWorkspaceEventRepository : Repository<RoadmapWorkspaceEvent>, IRoadmapWorkspaceEventRepository
{
    public RoadmapWorkspaceEventRepository(SkillMapDbContext context) : base(context) { }

    public async Task<int> GetLastAvailableEventVersion(long workspaceId, CancellationToken ct, bool withIncrement = false)
    {
        var lastEvent = await GetAllAsync(
            filter: e => e.RoadmapWorkspaceId == workspaceId,
            orderBy: q => q.OrderByDescending(e => e.CreatedAt).OrderByDescending(e => e.Version),
            count: 1,
            ct: ct);

        return (lastEvent.FirstOrDefault()?.Version ?? RoadmapWorkspaceConstants.InitialVersion) + (withIncrement ? 1 : 0);
    }

    public async Task<List<RoadmapWorkspaceEvent>> GetEventsAfter(long workspaceId, int version, CancellationToken ct)
       => (await GetAllAsync(
            filter: e => e.RoadmapWorkspaceId == workspaceId && e.Version > version,
            orderBy: q => q.OrderBy(e => e.CreatedAt).ThenBy(e => e.Version),
            ct: ct)).ToList();
    public async Task<List<RoadmapWorkspaceEvent>> GetEventsBetween(long workspaceId, int fromVersion, int toVersion, CancellationToken ct)
        => (await GetAllAsync(
            filter: e => e.RoadmapWorkspaceId == workspaceId && e.Version > fromVersion && e.Version <= toVersion,
            orderBy: q => q.OrderBy(e => e.CreatedAt).ThenBy(e => e.Version),
            ct: ct)).ToList();

    public async Task<bool> IsEventExist(long workspaceId, string idempotencyKey, CancellationToken ct)
        => await _dbSet.AnyAsync(e => e.RoadmapWorkspaceId == workspaceId && e.IdempotencyKey == idempotencyKey, ct);

}