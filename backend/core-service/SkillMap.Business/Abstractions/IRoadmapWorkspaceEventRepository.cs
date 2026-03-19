using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.Abstractions;
public interface IRoadmapWorkspaceEventRepository : IRepository<RoadmapWorkspaceEvent>
{
    Task<int> GetLastAvailableEventVersion(long workspaceId, CancellationToken ct, bool withIncrement = false);
}
