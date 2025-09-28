using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Results;
namespace LearningPlatform.Roadmap.Business.Contracts;
public interface IRoadmapService
{
    Task<Result<List<PlainRoadmapDto>>> GetPlainRoadmaps(int pageSize, int page, CancellationToken ct);
    //Task<Result<TreePlainResponse>> GetFullPlainRoadmap(string roadmapId, CancellationToken ct);
    //Task<Result<TreeResponse>> GetFullRoadmap(string roadmapId, CancellationToken ct);
    //Task<Result<List<RoadmapDto>>> GetRoadmaps(List<string> roadmapIds, CancellationToken ct);
}
