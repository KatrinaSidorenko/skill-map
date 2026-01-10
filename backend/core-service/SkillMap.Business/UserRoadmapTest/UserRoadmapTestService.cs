using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserTest;
using SkillMap.Core.Entities;
using SkillMap.Core.Entities.UserRoadmapTest;
using SkillMap.Shared.Results;
using System.Threading;

namespace SkillMap.Application.Services;

public class UserRoadmapTestService(IUnitOfWork unitOfWork) : IUserRoadmapTestService
{
    public async Task<string> SaveUserRoadmapTest(
        long userId,
        long userRoadmapId,
        string roadmapId,
        RoadmapTestType testType,
        RoadmapTestDao roadmapTest,
        CancellationToken ct)
    {
        roadmapTest.RoadmapId = roadmapId;

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

        await unitOfWork.SaveChangesAsync(ct);
        return entity.Id.ToString();
    }

    public async Task<RoadmapTestDao> GetUserRoadmapTest(
        long userId,
        string testId,
        CancellationToken ct)
    {
        var testIdL = long.Parse(testId); // todo: add check
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userRoadmapRepository = unitOfWork.CreateRepository<UserRoadmap>();
        var testResult = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(x => x.Id == testIdL, ct);

        if (!testResult.IsSuccessful || !testResult.HasData)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {testId} not found");
        var userRoadmapResult = await userRoadmapRepository.GetByIdAsync(testResult.Data.UserRoadmapId, ct);
        if (!userRoadmapResult.IsSuccessful || !userRoadmapResult.HasData || userRoadmapResult.Data.UserId != userId)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {testId} not found for user {userId}");

        var testEntity = testResult.Data;
        var testData = await testEntity.GetRoadmapTest(ct);
        return testData.ToDaoModel(testEntity.TestType, testEntity.UserRoadmapId.ToString(), testEntity.Id, userRoadmapResult.Data.RoadmapId);
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

        var roadmapTestResult = analysisResult.ToEntityResult();
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

    public async Task<(bool IsFound, long? RoadmapTestId)> HasRoadmapCompletedTest(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct)
    {
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userTestResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var userRoadmapTest = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapId == userRoadmapId && x.TestType == roadmapTestType.ToFriendlyString(),
            ct);
        if (!userRoadmapTest.HasData)
        {
            return (false, null);
        }
        var testResult = await userTestResultsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapTestId == userRoadmapTest.Data.Id,
            ct);
        if (!testResult.IsSuccessful)
        {
            return (false, null);
        }
        return (testResult.Data?.ResultData.Length > 0, userRoadmapTest.Data.Id);
    }

    public async Task<RoadmapTestResultsDto> GetLatestCompletedTestAnalysisResult(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct)
    {
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userTestResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var testResult = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapId == userRoadmapId && x.TestType == roadmapTestType.ToFriendlyString(),
            ct);
        if (!testResult.HasData)
        {
            return null;
        }

        var savedTestResult = await userTestResultsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapTestId == testResult.Data.Id,
            ct);
        if (!savedTestResult.IsSuccessful || !savedTestResult.HasData || savedTestResult.Data?.ResultData == null)
        {
            return null;
        }
        var testData = await savedTestResult.Data.GetTestResults(ct);
        return testData.ToDaoResult();
    }
}
