using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Application.InPorts.Retriever;

public interface IRetriever
{
    Task<Result<List<NodeDto>>> GetAllRoadmaps(CancellationToken ct = default);
    Task<Result<(List<NodeDto> Nodes, List<EdgeDto<NodeDto>> Edges)>> RetrieveByIdAsync(string roadmapId, CancellationToken ct = default);
}
