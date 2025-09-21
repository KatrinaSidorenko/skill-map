using SkillMap.Business.UserRoadmaps.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.UserRoadmaps;

public interface IUserRoadmapsService
{
    Task<Result<bool>> LinkRoadmap(long userId, string roadmapId, CancellationToken ct);
    Task<Result<List<UserRoadmapDto>>> GetUserRoadmaps(long userId, CancellationToken ct);
    Task<Result<bool>> RemoveRoadmap(long userId, string roadmapId, CancellationToken ct);
    Task<Result<UserRoadmapDto>> GetUserRoadmap(long userId, string roadmapId, CancellationToken ct);
}
