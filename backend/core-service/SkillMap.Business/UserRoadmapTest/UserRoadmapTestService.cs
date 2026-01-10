using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserTest;
using SkillMap.Core.Entities;
using SkillMap.Core.Entities.UserRoadmapTest;
using SkillMap.Shared.Extensions;
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

    public async Task<string> SaveStartOfTakingRoadmapTest(string roadmapTestId, CancellationToken ct)
    {
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var testIdL = long.Parse(roadmapTestId); // todo: add check
        var roadmapTest = await userRoadmapTestsRepository.GetByIdAsync(testIdL, ct);
        if (!roadmapTest.IsSuccessful || !roadmapTest.HasData)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {roadmapTestId} not found");

        // by simple path. assume that in db are any started results for this test
        var userTestResult = new UserTestResult
        {
            UserRoadmapTestId = roadmapTest.Data.Id,
            StartedAt = DateTime.UtcNow,
        };

        var userTestResultRepository = unitOfWork.CreateRepository<UserTestResult>();
        var addResult = await userTestResultRepository.AddAsync(userTestResult, ct);
        if (!addResult.IsSuccessful)
            throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, "Failed to save test start time");
        await unitOfWork.SaveChangesAsync(ct);
        return userTestResult.Id.ToString();
    }

    public async Task<RoadmapTestDao> GetRoadmapTest(string testId, CancellationToken ct)
    {
        var testIdL = long.Parse(testId); // todo: add check
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userRoadmapRepository = unitOfWork.CreateRepository<UserRoadmap>();
        var testResult = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(x => x.Id == testIdL, ct);

        if (!testResult.IsSuccessful || !testResult.HasData)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {testId} not found");
        var userRoadmapResult = await userRoadmapRepository.GetByIdAsync(testResult.Data.UserRoadmapId, ct);
        if (!userRoadmapResult.IsSuccessful || !userRoadmapResult.HasData)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {testId}");

        var testEntity = testResult.Data;
        var testData = await testEntity.GetRoadmapTest(ct);
        return testData.ToDaoModel(testEntity.TestType, testEntity.UserRoadmapId.ToString(), testEntity.Id, userRoadmapResult.Data.RoadmapId);
    }

    public async Task<string> SaveEndOfTakingRoadmapTestWithAnalysis(string roadmapTestId, RoadmapTestResultsDto analysisResult, CancellationToken ct)
    {
        var testIdL = long.Parse(roadmapTestId);
        var userTestResultRepository = unitOfWork.CreateRepository<UserTestResult>();
        var roadmapTestResult = analysisResult.ToDomainTestResult();
        var existingTestResult = await userTestResultRepository.GetAllAsync(
            filter: x => x.UserRoadmapTestId == testIdL,
            orderBy: q => q.OrderByDescending(x => x.StartedAt), 
            pageNum: null,
            count: 1, 
            ct);
        if (!existingTestResult.IsSuccessful || !existingTestResult.HasData || !existingTestResult.Data.Any())
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No started test result found for roadmap test id {roadmapTestId}");
        }

        var resultEntity = existingTestResult.Data.First();
        await resultEntity.SetTestResults(roadmapTestResult, ct);

        var updatingResult = await userTestResultRepository.UpdateAsync(resultEntity, ct);
        if (!updatingResult.IsSuccessful)
            throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, "Failed to save test result");

        await userTestResultRepository.SaveChangesAsync(ct);
        return resultEntity.Id.ToString();
    }

    public async Task<TestEstimationResult> GetRoadmapTestAnalysisResult(string testResultId, CancellationToken ct)
    {
        var testResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var testResultIdL = testResultId.EnsureParseLong();
        var testResult = await testResultsRepository.GetByIdAsync(testResultIdL, ct);
        if (!testResult.IsSuccessful || !testResult.HasData || testResult.Data?.ResultData == null)
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No results found for test result with id {testResultId}");

        var testData = await testResult.Data.GetTestResults(ct);
        var roadmapTestId = testResult.Data.UserRoadmapTestId;
        var roadmapTest = await GetRoadmapTest(roadmapTestId.ToString(), ct);
        return testData.ToDaoResult().ToTestEstimationResult(roadmapTest);
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
        var testResult = await userTestResultsRepository.GetFirstOrDefaultAsync(x => x.UserRoadmapTestId == userRoadmapTest.Data.Id, ct);
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

   // public async Task<List<RoadmapTestResultDto>
}
