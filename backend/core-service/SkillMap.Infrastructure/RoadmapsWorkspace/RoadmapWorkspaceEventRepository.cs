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

    public async Task<List<RoadmapWorkspaceEvent>> GetCheckedEventsGreaterThan(long workspaceId, int version, CancellationToken ct)
       => (await GetAllAsync(
     filter: e => e.RoadmapWorkspaceId == workspaceId && e.EventStatus == WorkspaceEventStatus.Applied && e.Version > version,
              orderBy: q => q.OrderBy(e => e.CreatedAt).ThenBy(e => e.Version),
    ct: ct)).ToList();

    public async Task<RoadmapWorkspaceEvent?> DequeueNextPendingEventAsync(CancellationToken ct)
    {
        return await _dbSet
            .FromSqlRaw("""
            SELECT * FROM workspace_event
             WHERE event_status = 'Pending'
             ORDER BY version ASC
             LIMIT 1
             FOR UPDATE SKIP LOCKED
     """)
       .FirstOrDefaultAsync(ct);
    }

    public async Task<List<RoadmapWorkspaceEvent>> GetAppliedEventsBetweenAsync(
        long workspaceId,
        int fromVersionExclusive,
        int toVersionExclusive,
        CancellationToken ct)
        => (await GetAllAsync(
                filter: e => e.RoadmapWorkspaceId == workspaceId
                && e.EventStatus == WorkspaceEventStatus.Applied
                && e.Version > fromVersionExclusive
                && e.Version < toVersionExclusive,
                orderBy: q => q.OrderBy(e => e.Version),
                ct: ct)).ToList();
}