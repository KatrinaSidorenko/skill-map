using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Business.RoadmapTest.Models;
using System;

namespace SkillMap.Business.UserTest;

public interface IUserTestService
{
    Task<RoadmapTestDao> GetUserTest(long userId, string testId, CancellationToken ct);
    Task SaveTestAnalysisResult(long userId, string testId, TestAnswersAnalysis analysisResult, CancellationToken ct);
    Task SaveUserTest(long userId, string roadmapId, RoadmapTestType testType, RoadmapTestDao roadmapTest, CancellationToken ct);
}
