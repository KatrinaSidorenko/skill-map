using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Results;

namespace LearningPlatform.Roadmap.Business;

public class RoadmapService(IRoadmapRepository roadmapRepository) : IRoadmapService
{
    public async Task<Result<List<PlainRoadmapDto>>> GetPlainRoadmaps(int pageSize, int page, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
