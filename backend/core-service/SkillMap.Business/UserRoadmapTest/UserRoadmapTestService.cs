using System.Threading;

using LearningPlatform.RoadmapTests.Contracts;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmapTest.Models;
using SkillMap.Business.UserTest;
using SkillMap.Core.Entities;
using SkillMap.Core.Entities.UserRoadmapTest;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

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
        await unitOfWork.SaveChangesAsync(ct);
        return entity.Id.ToString();
    }

    public async Task<string> SaveStartOfTakingRoadmapTest(string roadmapTestId, CancellationToken ct)
    {
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var testIdL = roadmapTestId.EnsureParseLong();
        var roadmapTest = await userRoadmapTestsRepository.GetByIdAsync(testIdL, ct);
        var userTestResultRepository = unitOfWork.CreateRepository<UserTestResult>();
        var existingTestResult = await userTestResultRepository.GetAllAsync(
            filter: x => x.UserRoadmapTestId == roadmapTest.Id && !x.CompletedAt.HasValue,
            orderBy: q => q.OrderByDescending(x => x.StartedAt),
            pageNum: null,
            count: 1,
            ct);
        if (existingTestResult.Any())
        {
            return existingTestResult.First().Id.ToString();
        }

        // by simple path. assume that in db are any started results for this test
        var userTestResult = new UserTestResult
        {
            UserRoadmapTestId = roadmapTest.Id,
            StartedAt = DateTime.UtcNow,
        };

        var addResult = await userTestResultRepository.AddAsync(userTestResult, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return userTestResult.Id.ToString();
    }

    public async Task<RoadmapTestDao> GetRoadmapTest(string testId, CancellationToken ct)
    {
        var testIdL = testId.EnsureParseLong();
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userRoadmapRepository = unitOfWork.CreateRepository<UserRoadmap>();
        var testEntity = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(x => x.Id == testIdL, ct);
        var userRoadmapResult = await userRoadmapRepository.GetByIdAsync(testEntity.UserRoadmapId, ct);
        var testData = await testEntity.GetRoadmapTest(ct);
        return testData.ToDaoModel(testEntity.TestType, testEntity.UserRoadmapId.ToString(), testEntity.Id, userRoadmapResult.RoadmapId);
    }

    public async Task<string> SaveEndOfTakingRoadmapTestWithAnalysis(string roadmapTestId, RoadmapTestResultsDto analysisResult, CancellationToken ct)
    {
        var testIdL = long.Parse(roadmapTestId);
        var userTestResultRepository = unitOfWork.CreateRepository<UserTestResult>();
        var roadmapTestResult = analysisResult.ToDomainTestResult();
        var existingTestResult = await userTestResultRepository.GetAllAsync(
            filter: x => x.UserRoadmapTestId == testIdL && !x.CompletedAt.HasValue,
            orderBy: q => q.OrderByDescending(x => x.StartedAt),
            pageNum: null,
            count: 1,
            ct);
        if (!existingTestResult.Any())
        {
            throw new LearningPlatformException(ErrorCode.NOTFOUND, $"No started test result found for roadmap test id {roadmapTestId}");
        }

        var resultEntity = existingTestResult.First();
        resultEntity.CompletedAt = DateTime.UtcNow;
        await resultEntity.SetTestResults(roadmapTestResult, ct);

        var updatingResult = await userTestResultRepository.UpdateAsync(resultEntity, ct);
        await userTestResultRepository.SaveChangesAsync(ct);
        return resultEntity.Id.ToString();
    }

    public async Task<TestEstimationResult> GetRoadmapTestAnalysisResult(string testResultId, CancellationToken ct)
    {
        var testResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var testResultIdL = testResultId.EnsureParseLong();
        var testResult = await testResultsRepository.GetByIdAsync(testResultIdL, ct);
        var testData = await testResult.GetTestResults(ct);
        var roadmapTestId = testResult.UserRoadmapTestId;
        var roadmapTest = await GetRoadmapTest(roadmapTestId.ToString(), ct);
        return testData.ToDaoResult().ToTestEstimationResult(roadmapTest);
    }

    public async Task<RoadmapTestResultsDto> GetLatestCompletedTestAnalysisResult(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct)
    {
        var userRoadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var userTestResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var testResult = await userRoadmapTestsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapId == userRoadmapId && x.TestType == roadmapTestType.ToFriendlyString(),
            ct);
        if (testResult == null)
        {
            return null;
        }

        var savedTestResult = await userTestResultsRepository.GetFirstOrDefaultAsync(
            x => x.UserRoadmapTestId == testResult.Id,
            ct);
        if (savedTestResult?.ResultData == null)
        {
            return null;
        }
        var testData = await savedTestResult.GetTestResults(ct);
        return testData.ToDaoResult();
    }

    public async Task<TestingHistoryDto> GetRoadmapTestingHistory(long userId, string roadmapId, CancellationToken ct)
    {
        var roadmapTestsRepository = unitOfWork.CreateRepository<UserRoadmapTest>();
        var testResultsRepository = unitOfWork.CreateRepository<UserTestResult>();
        var userRoadmapRepository = unitOfWork.CreateRepository<UserRoadmap>();
        var userRoadmap = await userRoadmapRepository.GetFirstOrDefaultAsync(filter: x => x.UserId == userId && x.RoadmapId == roadmapId, ct);
        var roadmapTests = await roadmapTestsRepository.GetAllAsync(filter: x => x.UserRoadmapId == userRoadmap.Id, ct: ct);
        var targetRoadmapTestIds = roadmapTests.Select(x => x.Id).ToHashSet();

        var testResults = await testResultsRepository.GetAllAsync(filter: x => targetRoadmapTestIds.Contains(x.UserRoadmapTestId), orderBy: q => q.OrderByDescending(x => x.StartedAt), ct: ct);
        var testResultsByRoadmapTestId = testResults.GroupBy(x => x.UserRoadmapTestId).ToDictionary(g => g.Key, g => g.ToList());

        var testingHistory = roadmapTests.OrderBy(t => t.CreatedAt).Select(t =>
        {
            var results = testResultsByRoadmapTestId.GetOrDefault(t.Id);
            var testAttemptsResults = results?.Select(r => new TestAttemptDto
            {
                ResultId = r.Id.ToString(),
                StartedAt = r.StartedAt,
                CompletedAt = r.CompletedAt,
                Score = r.ScoredPoints
            }).ToList() ?? new List<TestAttemptDto>();

            var maxPoints = results?.FirstOrDefault()?.MaxPoints ?? 0; // todo: fills like it should be stored in UserRoadmapTest entity
            return new TestHistoryItemDto
            {
                TestId = t.Id.ToString(),
                MaxScore = maxPoints,
                Type = t.TestType,
                Attempts = testAttemptsResults
            };
        }).ToList();

        return new TestingHistoryDto
        {
            Items = testingHistory
        };
    }
}