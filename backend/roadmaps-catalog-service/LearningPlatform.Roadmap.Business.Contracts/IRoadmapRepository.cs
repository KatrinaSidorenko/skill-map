using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace LearningPlatform.Roadmap.Business.Contracts;

public interface IRoadmapRepository
{
    Task<bool> Save((List<NodeDto> Nodes, List<EdgeDto> Edges) graph, CancellationToken cancellationToken = default);
    Task<Result<(List<NodeDto> Nodes, List<EdgeDto> Edges)>> GetRoadmapById(string roadmapId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AddNodes(List<NodeDto> nodes, CancellationToken ct = default);
    Task<Result<bool>> AddEdges(List<EdgeDto> edges, CancellationToken ct = default);
    Task<Result<PaginationResult<List<NodeDto>>>> GetAllPlainRoadmaps(SearchingParams @params, CancellationToken ct);
    Task<Result<PaginationResult<List<NodeDto>>>> GetPlainRoadmapsByIds(List<string> roadmapIds, SearchingParams @params, CancellationToken ct);
    Task<Result<Dictionary<string, int>>> CalculateTotalTopicsAndSubtopics(List<string> roadmapIds, CancellationToken ct);
    Task<Result<List<ResourceDto>>> GetRoadmapItemMaterials(string roadmapId, string itemId, CancellationToken ct);
}
