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
    public async Task<string> SaveUserTestWithEmptyResult(
        long userId,
        long userRoadmapId,
        string roadmapId,
        RoadmapTestType testType,
        RoadmapTestDao roadmapTest,
        CancellationToken ct)
    {
        roadmapTest.RoadmapId = roadmapId;
        //roadmapTest.Id = $"{userId}_{roadmapId}"; // todo: Guid id

        var entity = new UserRoadmapTest
        {
            UserRoadmapId = userRoadmapId,
            TestType = testType.ToFriendlyString(),
        };
        await entity.SetRoadmapTest(roadmapTest.ToEntityModel(), ct);
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var addResult = await userRoadmapTestsRepository.AddAsync(entity, ct);
        if (!addResult.IsSuccessful)
            throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, "Failed to save user test");

        var userTestResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var resultEntity = new UserTestResult
        {
            MaxPoints = 0,
            ScoredPoints = 0,
            ResultData = null,
            UserRoadmapTest = entity,
        };

        addResult = await userTestResultsRepository.AddAsync(resultEntity, ct);
        if (!addResult.IsSuccessful)
            throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, "Failed to save user test result");

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
        var testData = await testEntity.GetRoadmapTest(ct);
        return testData.ToDaoModel(testEntity.TestType, testEntity.UserRoadmapId.ToString(), testEntity.Id);
    }

    public async Task SaveTestAnalysisResult(
        long userRoadmapId,
        string testId,
        RoadmapTestResultsDto analysisResult,
        CancellationToken ct)
    {
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userTestResultRepository = unitOfWork.CreateRepository<UserTestResult>();
        var testEntity = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapId == userRoadmapId,
            ct);

        if (!testEntity.IsSuccessful || !testEntity.HasData)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No test found for user roadmap {userRoadmapId}");

        // try to find existing result
        var roadmapTestResult = analysisResult.ToEntityResult();

        var existingResult = await userTestResultRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapTestId == testEntity.Data.Id,
            ct);
        if (existingResult.IsSuccessful && existingResult.HasData)
        {
            existingResult.Data.CompletedAt = DateTime.UtcNow;
            await existingResult.Data.SetTestResults(roadmapTestResult, ct);
            var updateResult = await userTestResultRepository.UpdateAsync(existingResult.Data, ct);
            if (!updateResult.IsSuccessful)
                throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, "Failed to update test result");
            await userTestResultRepository.SaveChangesAsync(ct);
            return;
        }

        var resultEntity = new UserTestResult
        {
            UserRoadmapTestId = testEntity.Data.Id,
            MaxPoints = analysisResult.TotalPossiblePoints,
            ScoredPoints = analysisResult.AchievedPoints,
            CompletedAt = DateTime.UtcNow,
        };

        await resultEntity.SetTestResults(roadmapTestResult, ct);

        var addResult = await userTestResultRepository.AddAsync(resultEntity, ct);
        if (!addResult.IsSuccessful)
            throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, "Failed to save test result");

        await userTestResultRepository.SaveChangesAsync(ct);
    }

    public async Task<RoadmapTestResultsDto> GetTestAnalysisResult(
        long userId,
        string testId,
        CancellationToken ct)
    {
        var testIdL = long.Parse(testId); // todo: add check
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userTestResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var testResult = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(x => x.Id == testIdL, ct);
        if (!testResult.IsSuccessful || !testResult.HasData)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {testId} not found");
        var savedTestResult = await userTestResultsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapTestId == testResult.Data.Id,
            ct);
        if (!savedTestResult.IsSuccessful || !savedTestResult.HasData || savedTestResult.Data?.ResultData == null)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No results found for test with id {testId}");
        
        var testData = await savedTestResult.Data.GetTestResults(ct);
        return testData.ToDaoResult();
    }

    public async Task<(bool IsFound, RoadmapTestDao? Test)> ExistsUnfinishedTest(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct)
    {
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userTestResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var testResult = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapId == userRoadmapId && x.TestType == roadmapTestType.ToFriendlyString(),
            ct);
        if (!testResult.HasData)
        {
            return (false, null);
        }

        var savedTestResult = await userTestResultsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapTestId == testResult.Data.Id,
            ct);
        if (!savedTestResult.IsSuccessful)
        {
            return (false, null); // something went wrong
        }

        var testData = await testResult.Data.GetRoadmapTest(ct);
        return (savedTestResult.Data?.ResultData.Length > 0, testData.ToDaoModel(testResult.Data.TestType, testResult.Data.UserRoadmapId.ToString(), testResult.Data.Id));
    }
}
