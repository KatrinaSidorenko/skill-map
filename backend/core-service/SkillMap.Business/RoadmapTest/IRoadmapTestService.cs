using SkillMap.Business.RoadmapTest.Models;

namespace SkillMap.Business.RoadmapTest;

public interface IRoadmapTestService
{
    Task<ComplexTestCheckResult> CheckRoadmapTest(long userId, string testId, RoadmapTestAnswers userAnswers, CancellationToken ct);
    Task<RoadmapTestResultDto> GenerateRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct);
}
