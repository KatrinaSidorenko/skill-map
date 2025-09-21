using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Application.OutPorts.Persistence;

public interface IRoadmapRepository
{
    Task<bool> Save(Graph graph, CancellationToken cancellationToken = default);
    Task<Result<Graph>> GetRoadmap(string roadmapId, CancellationToken cancellationToken = default);
    Task<Result<List<NodeDto>>> GetAllRoadmaps(CancellationToken ct);
    Task<Result<bool>> UpdateNodesDescription(Dictionary<string, string> nodesPropsToUpdate, CancellationToken ct);
    Task<Result<bool>> AddNodes(List<NodeDto> nodes, CancellationToken ct = default);
    Task<Result<bool>> AddEdges(List<EdgeDto<NodeDto>> edges, CancellationToken ct = default);
}
