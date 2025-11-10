using LearningPlatform.RoadmapTests.Contracts;
using Microsoft.Extensions.Caching.Memory;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.UserTest;

public class UserTestService(IMemoryCache memoryCache) : IUserTestService
{
    private const string RoadmapTestCacheKeyPrefix = "RoadmapTest_";

    // todo: test type
    public async Task<string> SaveUserTest(long userId, string roadmapId, RoadmapTestType testType, RoadmapTestDao roadmapTest, CancellationToken ct)
    {
        // create time save
        // save test type
        roadmapTest.RoadmapId = roadmapId;
        roadmapTest.Id = $"{userId}_{roadmapId}";
        memoryCache.Set(RoadmapTestCacheKeyPrefix + roadmapTest.Id, roadmapTest, TimeSpan.FromHours(1));
        return await Task.FromResult(roadmapTest.Id);
    }

    public async Task<RoadmapTestDao> GetUserTest(long userId, string testId, CancellationToken ct)
    {
        //todo: get test from db or cache
        if (!memoryCache.TryGetValue<RoadmapTestDao>(RoadmapTestCacheKeyPrefix + testId, out var roadmapTest))
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {testId} not found");
        }
        return await Task.FromResult(roadmapTest);
    }

    public async Task SaveTestAnalysisResult(long userId, string testId, RoadmapTestResultsDto analysisResult, CancellationToken ct)
    {

    }
}
