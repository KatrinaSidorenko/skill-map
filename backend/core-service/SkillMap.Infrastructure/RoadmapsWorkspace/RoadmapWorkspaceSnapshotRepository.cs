using System.Threading;

using Org.BouncyCastle.Asn1.Ocsp;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Persistence;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class RoadmapWorkspaceSnapshotRepository : Repository<RoadmapWorkspaceSnapshot>, IRoadmapWorkspaceSnapshotRepository
{
    public RoadmapWorkspaceSnapshotRepository(SkillMapDbContext context) : base(context) { }

    public async Task<RoadmapWorkspaceSnapshot?> GetLatestSnapshot(long workspaceId, CancellationToken ct)
    {
        return (await GetAllAsync(
            filter: s => s.RoadmapWorkspaceId == workspaceId,
            orderBy: q => q.OrderByDescending(s => s.CreatedAt).ThenByDescending(s => s.UpdatedAt),
            count: 1,
            ct: ct)).FirstOrDefault();
    }
}