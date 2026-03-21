using System.Threading;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Core.PersonalizedRoadmaps;
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
}
