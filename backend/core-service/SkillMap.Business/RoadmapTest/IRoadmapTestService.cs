using SkillMap.Business.RoadmapTest.Models;

namespace SkillMap.Business.RoadmapTest;

public interface IRoadmapTestService
{
    Task<RoadmapTestResult> GenerateRoadmapTest(long userId, string roadmapId, RoadmapTestConfig config, CancellationToken ct);
}
