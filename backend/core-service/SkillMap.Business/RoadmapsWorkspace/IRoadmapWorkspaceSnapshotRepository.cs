using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapWorkspaceSnapshotRepository : IRepository<RoadmapWorkspaceSnapshot>
{
    Task<RoadmapWorkspaceSnapshot> GetLatestSnapshot(long workspaceId, CancellationToken ct);
}