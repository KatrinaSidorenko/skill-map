using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;
namespace LearningPlatform.Roadmap.Business.Contracts;
public interface IRoadmapService
{
    Task<Result<PaginationResult<List<PlainRoadmapDto>>>> GetPlainRoadmaps(PaginationParams paginationParams, CancellationToken ct);
}
