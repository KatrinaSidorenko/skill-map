using System;

using LearningPlatform.RoadmapTests.Contracts;

using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmapTest.Models;

namespace SkillMap.Business.UserTest;

public interface IAssessmentAttemptService
{
    Task<RoadmapTestResultsDto> GetLatestCompletedTestAnalysisResult(long userRoadmapId, RoadmapAssessmentType roadmapTestType, CancellationToken ct);
    Task<string> SaveStartOfTakingRoadmapTest(string roadmapTestId, CancellationToken ct);
    Task<string> SaveEndOfTakingRoadmapTestWithAnalysis(string roadmapTestId, RoadmapTestResultsDto analysisResult, CancellationToken ct);
    Task<RoadmapTestDao> GetRoadmapTest(string testId, CancellationToken ct);
    Task<TestEstimationResult> GetRoadmapTestAnalysisResult(string testResultId, CancellationToken ct);
    Task<TestingHistoryDto> GetRoadmapTestingHistory(long workspaceId, CancellationToken ct);
    Task<string> SaveUserRoadmapTest(long workspaceId, string roadmapId, RoadmapAssessmentType testType, RoadmapTestDao roadmapTest, CancellationToken ct);
}