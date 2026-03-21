using System.Threading;

using LearningPlatform.RoadmapTests.Contracts;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmapTest.Models;
using SkillMap.Business.UserTest;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Application.AssessmentAttempts;

public class AssessmentAttemptService(IUnitOfWork unitOfWork) : IAssessmentAttemptService
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

        var entity = new RoadmapAssessment
        {
            RoadmapWorkspaceId = userRoadmapId,
            TestType = testType.ToFriendlyString(),
        };
        await entity.SetRoadmapTest(roadmapTest.ToEntityModel(), ct);

        var assessmentRepository = unitOfWork.CreateRepository<RoadmapAssessment>();
        await assessmentRepository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return entity.Id.ToString();
    }

    public async Task<string> SaveStartOfTakingRoadmapTest(string roadmapTestId, CancellationToken ct)
    {
        var assessmentRepository = unitOfWork.CreateRepository<RoadmapAssessment>();
        var testIdL = roadmapTestId.EnsureParseLong();
        var assessment = await assessmentRepository.GetByIdAsync(testIdL, ct);

        var attemptRepository = unitOfWork.CreateRepository<AssessmentAttempt>();
        var existingAttempt = await attemptRepository.GetAllAsync(
            filter: x => x.AssessmentId == assessment.Id && !x.CompletedAt.HasValue,
            orderBy: q => q.OrderByDescending(x => x.StartedAt),
            pageNum: null,
            count: 1,
            ct);
        if (existingAttempt.Any())
        {
            return existingAttempt.First().Id.ToString();
        }

        var attempt = new AssessmentAttempt
        {
            AssessmentId = assessment.Id,
            StartedAt = DateTime.UtcNow,
        };

        await attemptRepository.AddAsync(attempt, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return attempt.Id.ToString();
    }

    public async Task<RoadmapTestDao> GetRoadmapTest(string testId, CancellationToken ct)
    {
        var testIdL = testId.EnsureParseLong();
        var assessmentRepository = unitOfWork.CreateRepository<RoadmapAssessment>();
        var workspaceRepository = unitOfWork.CreateRepository<RoadmapWorkspace>();

        var assessment = await assessmentRepository.GetFirstOrDefaultAsync(x => x.Id == testIdL, ct);
        var workspace = await workspaceRepository.GetByIdAsync(assessment.RoadmapWorkspaceId, ct);
        var testData = await assessment.GetRoadmapTest(ct);
        return testData.ToDaoModel(assessment.TestType, assessment.RoadmapWorkspaceId.ToString(), assessment.Id, workspace.ActualRoadmapId);
    }

    public async Task<string> SaveEndOfTakingRoadmapTestWithAnalysis(string roadmapTestId, RoadmapTestResultsDto analysisResult, CancellationToken ct)
    {
        var testIdL = long.Parse(roadmapTestId);
        var attemptRepository = unitOfWork.CreateRepository<AssessmentAttempt>();
        var domainResult = analysisResult.ToDomainTestResult();

        var existingAttempt = await attemptRepository.GetAllAsync(
            filter: x => x.AssessmentId == testIdL && !x.CompletedAt.HasValue,
            orderBy: q => q.OrderByDescending(x => x.StartedAt),
            pageNum: null,
            count: 1,
            ct);
        if (!existingAttempt.Any())
        {
            throw new LearningPlatformException(ErrorCode.NOTFOUND, $"No started test result found for roadmap test id {roadmapTestId}");
        }

        var attempt = existingAttempt.First();
        attempt.CompletedAt = DateTime.UtcNow;
        await attempt.SetTestResults(domainResult, ct);

        await attemptRepository.UpdateAsync(attempt, ct);
        await attemptRepository.SaveChangesAsync(ct);
        return attempt.Id.ToString();
    }

    public async Task<TestEstimationResult> GetRoadmapTestAnalysisResult(string testResultId, CancellationToken ct)
    {
        var attemptRepository = unitOfWork.CreateRepository<AssessmentAttempt>();
        var testResultIdL = testResultId.EnsureParseLong();
        var attempt = await attemptRepository.GetByIdAsync(testResultIdL, ct);
        var testData = await attempt.GetTestResults(ct);
        var roadmapTest = await GetRoadmapTest(attempt.AssessmentId.ToString(), ct);
        return testData.ToDaoResult().ToTestEstimationResult(roadmapTest);
    }

    public async Task<RoadmapTestResultsDto> GetLatestCompletedTestAnalysisResult(long userRoadmapId, RoadmapTestType roadmapTestType, CancellationToken ct)
    {
        var assessmentRepository = unitOfWork.CreateRepository<RoadmapAssessment>();
        var attemptRepository = unitOfWork.CreateRepository<AssessmentAttempt>();

        var assessment = await assessmentRepository.GetFirstOrDefaultAsync(
            x => x.RoadmapWorkspaceId == userRoadmapId && x.TestType == roadmapTestType.ToFriendlyString(),
            ct);
        if (assessment == null)
        {
            return null;
        }

        var savedAttempt = await attemptRepository.GetFirstOrDefaultAsync(
            x => x.AssessmentId == assessment.Id,
            ct);
        if (savedAttempt?.ResultData == null)
        {
            return null;
        }

        var testData = await savedAttempt.GetTestResults(ct);
        return testData.ToDaoResult();
    }

    public async Task<TestingHistoryDto> GetRoadmapTestingHistory(long workspaceId, CancellationToken ct)
    {
        var assessmentRepository = unitOfWork.CreateRepository<RoadmapAssessment>();
        var attemptRepository = unitOfWork.CreateRepository<AssessmentAttempt>();
        var workspaceRepository = unitOfWork.CreateRepository<RoadmapWorkspace>();

        var workspace = await workspaceRepository.GetFirstOrDefaultAsync(w => w.Id == workspaceId, ct)
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), $"Workspace with id {workspaceId} not found");

        var assessments = workspace.Assessments ?? [];
        var assessmentIds = assessments.Select(x => x.Id).ToHashSet();
        var attempts = await attemptRepository.GetAllAsync(
            filter: x => assessmentIds.Contains(x.AssessmentId),
            orderBy: q => q.OrderByDescending(x => x.StartedAt),
            ct: ct);

        var attemptsByAssessmentId = attempts
            .GroupBy(x => x.AssessmentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var testingHistory = assessments.OrderBy(a => a.CreatedAt).Select(a =>
        {
            var assessmentAttempts = attemptsByAssessmentId.GetOrDefault(a.Id);
            var testAttemptsResults = assessmentAttempts?.Select(r => new TestAttemptDto
            {
                ResultId = r.Id.ToString(),
                StartedAt = r.StartedAt,
                CompletedAt = r.CompletedAt,
                Score = r.ScoredPoints
            }).ToList() ?? [];

            var maxPoints = assessmentAttempts?.FirstOrDefault()?.MaxPoints ?? 0;
            return new TestHistoryItemDto
            {
                TestId = a.Id.ToString(),
                MaxScore = maxPoints,
                Type = a.TestType,
                Attempts = testAttemptsResults
            };
        }).ToList();

        return new TestingHistoryDto { Items = testingHistory };
    }
}