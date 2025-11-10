using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserTest;
using SkillMap.Core.Entities.UserRoadmapTest;
using SkillMap.Shared.Results;

namespace SkillMap.Application.Services;

public class UserTestService(IUnitOfWork unitOfWork) : IUserTestService
{

    // todo: when add the test create the test result too
    public async Task<string> SaveUserTest(
        long userId,
        string roadmapId,
        RoadmapTestType testType,
        RoadmapTestDao roadmapTest,
        CancellationToken ct)
    {
        // todo: get user roadmap by user and roadmap id to set the foreign key
        roadmapTest.RoadmapId = roadmapId;
        roadmapTest.Id = $"{userId}_{roadmapId}"; // todo: Guid id

        var entity = new UserRoadmapTest
        {
            UserRoadmapId = long.Parse(roadmapId),
            TestType = testType.ToFriendlyString(),
            TestData = roadmapTest.ToEntityModel(),
        };

        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var addResult = await userRoadmapTestsRepository.AddAsync(entity, ct);
        if (!addResult.IsSuccessful)
            throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, "Failed to save user test");

        await unitOfWork.SaveChangesAsync(ct);
        return entity.Id.ToString();
    }

    public async Task<RoadmapTestDao> GetUserTest(
        long userId,
        string testId,
        CancellationToken ct)
    {
        var testIdL = long.Parse(testId); // todo: add check
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var testResult = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(x => x.Id == testIdL, ct);

        if (!testResult.IsSuccessful || !testResult.HasData)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {testId} not found");
        var testEntity = testResult.Data;
        return testEntity.TestData.ToDaoModel(testEntity.TestType, testEntity.UserRoadmapId.ToString());
    }

    public async Task SaveTestAnalysisResult(
        long userId,
        string testId,
        RoadmapTestResultsDto analysisResult,
        CancellationToken ct)
    {
        //var testEntity = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(
        //    x => x.UserRoadmapId == userId,
        //    ct);

        //if (!testEntity.IsSuccess || testEntity.Value == null)
        //    throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No test found for user {userId}");

        //var resultEntity = new UserTestResult
        //{
        //    UserRoadmapTestId = testEntity.Value.Id,
        //    MaxPoints = analysisResult.MaxPoints,
        //    ScoredPoints = analysisResult.ScoredPoints,
        //    ResultData = analysisResult.ToEntityResult(),
        //    CompletedAt = DateTime.UtcNow,
        //    CreatedAt = DateTime.UtcNow,
        //    UpdatedAt = DateTime.UtcNow
        //};

        //var addResult = await userTestResultsRepository.AddAsync(resultEntity, ct);
        //if (!addResult.IsSuccess)
        //    throw new LearningPlatformException(ErrorCode.DB_ERROR, "Failed to save test result");

        //await userTestResultsRepository.SaveChangesAsync(ct);
    }
}
