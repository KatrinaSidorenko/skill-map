using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Business.RoadmapTest.Models;
using System;

namespace SkillMap.Business.UserTest;

public interface IUserRoadmapTestService
{
    Task<string> SaveUserRoadmapTest(long userId, long userRoadmapId, string roadmapId, RoadmapTestType testType, RoadmapTestDao roadmapTest, CancellationToken ct);
    Task<RoadmapTestResultsDto> GetLatestCompletedTestAnalysisResult(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct);
    Task<(bool IsFound, long? RoadmapTestId)> HasRoadmapCompletedTest(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct);

    Task<string> SaveStartOfTakingRoadmapTest(string roadmapTestId, CancellationToken ct);
    Task<string> SaveEndOfTakingRoadmapTestWithAnalysis(string roadmapTestId, RoadmapTestResultsDto analysisResult, CancellationToken ct);
    Task<RoadmapTestDao> GetRoadmapTest(string testId, CancellationToken ct);
    Task<TestEstimationResult> GetRoadmapTestAnalysisResult(string testResultId, CancellationToken ct);
}
