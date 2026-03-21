using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapWorkspaceRepository : IRepository<RoadmapWorkspace>
{
    Task<List<RoadmapWorkspace>> GetUserBlueprintWorkspaces(long userId, CancellationToken ct);
    Task<bool> IsWorkspaceActive(string roadmapId, long userId, CancellationToken ct);
}