using SkillMap.Business.Roadmaps.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Abstractions;

public interface IRoadmapService
{
    Task<Result<TreePlainResponse>> GetFullPlainRoadmap(string roadmapId, CancellationToken ct);
    Task<Result<TreeResponse>> GetFullRoadmap(string roadmapId, CancellationToken ct);
    Task<Result<List<Roadmap>>> GetRoadmaps(List<string> roadmapIds, CancellationToken ct);
}
