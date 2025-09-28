using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Results;

namespace LearningPlatform.Roadmap.Business.Contracts;

public interface IRetriever
{
    Task<Result<List<NodeDto>>> GetAllRoadmaps(CancellationToken ct = default);
    Task<Result<(List<NodeDto> Nodes, List<EdgeDto> Edges)>> RetrieveByIdAsync(string roadmapId, CancellationToken ct = default);
}
