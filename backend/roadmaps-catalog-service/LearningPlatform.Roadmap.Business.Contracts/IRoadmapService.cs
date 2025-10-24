using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;
namespace LearningPlatform.Roadmap.Business.Contracts;
public interface IRoadmapService
{
    Task<Result<PaginationResult<List<PlainRoadmapDto>>>> GetPlainRoadmaps(SearchingParams @params, CancellationToken ct);
    Task<Result<RoadmapDto>> GetRoadmapById(string roadmapId, CancellationToken ct);
    Task<Result<List<ResourceDto>>> GetLearningItemMaterials(string roadmapId, string itemId, CancellationToken ct);
    Task CreateNode(NodeDto node, CancellationToken ct = default);
    Task CreateEdge(EdgeDto edge, CancellationToken ct = default);
    Task<Result<string>> CreateRoadmap(PlainRoadmapDto roadmapDto, CancellationToken ct);
    Task<Result<PaginationResult<List<PlainRoadmapDto>>>> GetPlainRoadmapsByIds(List<string> roadmapIds, SearchingParams @params, CancellationToken ct, bool excludePrivate = true);
}
