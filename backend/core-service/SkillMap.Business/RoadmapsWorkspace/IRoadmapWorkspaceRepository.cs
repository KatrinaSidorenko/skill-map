using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapWorkspaceRepository : IRepository<RoadmapWorkspace>
{
    Task<List<RoadmapWorkspace>> GetUserActiveNonAuthoredWorkspacesWithSnapshots(long userId, int pageNumber, int pageSize, CancellationToken ct);
    Task<RoadmapWorkspace> GetUserActiveWorkspace(long workspaceId, CancellationToken ct);
    Task<List<RoadmapWorkspace>> GetUserBlueprintWorkspaces(long userId, CancellationToken ct);
    Task<bool> IsWorkspaceActive(string roadmapId, long userId, CancellationToken ct);
}