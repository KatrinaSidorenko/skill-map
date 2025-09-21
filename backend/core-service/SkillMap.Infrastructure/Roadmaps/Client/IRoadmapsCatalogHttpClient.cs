using SkillMap.Business.Roadmaps.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Infrastructure.Roadmaps.Client;

public interface IRoadmapsCatalogHttpClient
{
    Task<Result<List<RoadmapDto>>> GetAllRoadmaps(CancellationToken ct);
    Task<Result<TreePlainResponse>> GetFullPlainRoadmap(string roadmapId, CancellationToken ct);
    Task<Result<TreeResponse>> GetFullRoadmap(string roadmapId, CancellationToken ct);
}
