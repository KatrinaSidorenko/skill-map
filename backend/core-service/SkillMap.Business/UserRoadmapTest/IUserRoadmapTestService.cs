using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Business.RoadmapTest.Models;
using System;

namespace SkillMap.Business.UserTest;

public interface IUserRoadmapTestService
{
    Task<RoadmapTestDao> GetUserRoadmapTest(long userId, string testId, CancellationToken ct);
    Task<RoadmapTestResultsDto> GetTestAnalysisResult(long userId, string testId, CancellationToken ct);
    Task SaveTestAnalysisResult(long userRoadmapId, string testId, RoadmapTestResultsDto analysisResult, CancellationToken ct);
    Task<string> SaveUserRoadmapTest(long userId, long userRoadmapId, string roadmapId, RoadmapTestType testType, RoadmapTestDao roadmapTest, CancellationToken ct);
    Task<RoadmapTestResultsDto> GetLatestCompletedTestAnalysisResult(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct);
    Task<(bool IsFound, long? RoadmapTestId)> HasRoadmapCompletedTest(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct);
}
