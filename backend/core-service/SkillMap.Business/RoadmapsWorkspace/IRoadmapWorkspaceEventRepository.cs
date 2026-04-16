using SkillMap.Business.Abstractions;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapWorkspaceEventRepository : IRepository<RoadmapWorkspaceEvent>
{
    Task<List<RoadmapWorkspaceEvent>> GetCheckedEventsGreaterThan(long workspaceId, int version, CancellationToken ct);
    Task<int> GetLastAvailableEventVersion(long workspaceId, CancellationToken ct, bool withIncrement = false);
    Task<bool> IsEventExist(long workspaceId, string idempotencyKey, CancellationToken ct);
}