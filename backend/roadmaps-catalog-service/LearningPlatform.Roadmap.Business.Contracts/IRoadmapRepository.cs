using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace LearningPlatform.Roadmap.Business.Contracts;

public interface IRoadmapRepository
{
    Task<bool> Save((List<NodeDto> Nodes, List<EdgeDto> Edges) graph, CancellationToken cancellationToken = default);
    Task<Result<(List<NodeDto> Nodes, List<EdgeDto> Edges)>> GetRoadmap(string roadmapId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateNodesDescription(Dictionary<string, string> nodesPropsToUpdate, CancellationToken ct);
    Task<Result<bool>> AddNodes(List<NodeDto> nodes, CancellationToken ct = default);
    Task<Result<bool>> AddEdges(List<EdgeDto> edges, CancellationToken ct = default);
    Task<Result<PaginationResult<List<NodeDto>>>> GetAllPlainRoadmaps(PaginationParams paginationParams, CancellationToken ct);
}
