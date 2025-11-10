using SkillMap.Business.RoadmapTest.Models;

namespace SkillMap.Business.RoadmapTest;

public interface IRoadmapTestService
{
    Task<AnswersCheckResult> CheckRoadmapTest(long userId, string testId, RoadmapTestAnswers userAnswers, CancellationToken ct);
    Task<RoadmapTestResult> GenerateRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct);
}
