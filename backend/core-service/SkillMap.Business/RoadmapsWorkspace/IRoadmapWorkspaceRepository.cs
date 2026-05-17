using System.Linq.Expressions;

using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapWorkspaceRepository : IRepository<RoadmapWorkspace>
{
    Task<List<RoadmapWorkspace>> GetUserActiveNonAuthoredWorkspacesWithProjections(long userId, int pageNumber, int pageSize, Expression<Func<RoadmapWorkspace, bool>> filter = null, Func<IQueryable<RoadmapWorkspace>, IOrderedQueryable<RoadmapWorkspace>> orderBy = null, CancellationToken ct = default);
    Task<RoadmapWorkspace> GetUserActiveWorkspace(long workspaceId, CancellationToken ct);
    Task<List<RoadmapWorkspace>> GetUserBlueprintWorkspaces(long userId, CancellationToken ct);
    Task<bool> IsWorkspaceActive(string roadmapId, long userId, CancellationToken ct);
}