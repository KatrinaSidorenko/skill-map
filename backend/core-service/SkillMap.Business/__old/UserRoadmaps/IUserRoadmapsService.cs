using LearningPlatform.Roadmap.Business.Contracts.Models;

using SkillMap.Business.__old.UserRoadmaps.Models;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.__old.UserRoadmaps;

public interface IUserRoadmapsService
{
    Task<Result<List<UserRoadmapDto>>> GetUserSavedRoadmaps(long userId, CancellationToken ct);
    Task<Result<bool>> RemoveRoadmap(long userId, string roadmapId, CancellationToken ct);
    Task<Result<bool>> LinkRoadmap(long userId, string roadmapId, CancellationToken ct, bool isOwner = false);
    Task<Result<UserRoadmapDto>> GetUserRoadmap(long userId, string roadmapId, CancellationToken ct, bool? isActive = null);


    Task<Result<PaginationResult<List<PlainRoadmapDto>>>> GetUserCreatedRoadmaps(long userId, SearchingParams @params, CancellationToken ct);
    Task<Result<string>> CreateUserRoadmap(long userId, PlainRoadmapDto roadmapDto, CancellationToken ct);
    Task<Result<PlainRoadmapDto>> GetCreatedUserRoadmap(long userId, string roadmapId, CancellationToken ct);
}