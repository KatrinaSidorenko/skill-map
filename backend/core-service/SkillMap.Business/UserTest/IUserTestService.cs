using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Business.RoadmapTest.Models;
using System;

namespace SkillMap.Business.UserTest;

public interface IUserTestService
{
    Task<(bool IsFound, RoadmapTestDao Test)> ExistsUnfinishedTest(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct);
    Task<RoadmapTestDao> GetUserTest(long userId, string testId, CancellationToken ct);
    Task<RoadmapTestResultsDto> GetTestAnalysisResult(long userId, string testId, CancellationToken ct);
    Task SaveTestAnalysisResult(long userRoadmapId, string testId, RoadmapTestResultsDto analysisResult, CancellationToken ct);
    Task<string> SaveUserTestWithEmptyResult(long userId, long userRoadmapId, string roadmapId, RoadmapTestType testType, RoadmapTestDao roadmapTest, CancellationToken ct);
}
