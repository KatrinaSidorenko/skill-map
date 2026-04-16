using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapAssessments.Features.GetAssessmentHistory;


[UsedImplicitly]
internal sealed class GetAssessmentHistoryHandler(
        IRoadmapWorkspaceRepository workspaceRepository,
        IRepository<RoadmapAssessment> assessmentRepository,
        IRepository<AssessmentAttempt> attemptRepository)
    : IRequestHandler<GetAssessmentHistoryQuery, AssessmentHistoryDto>
{
    public async Task<AssessmentHistoryDto> Handle(
        GetAssessmentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetUserActiveWorkspace(request.WorkspaceId, cancellationToken) ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), $"Roadmap workspace with id {request.WorkspaceId} not found.");
        var assessments = await assessmentRepository.GetAllAsync(
            filter: a => a.RoadmapWorkspaceId == request.WorkspaceId,
            orderBy: q => q.OrderByDescending(a => a.CreatedAt),
            ct: cancellationToken);
        var hasCompletedTopic = workspace.LearningItemProjections.Any(li => li.Status == LearningStatus.Completed.ToStatusString());

        if (!assessments.Any())
        {
            return new AssessmentHistoryDto(Items: [], IsIntermediateAssessmentAvailable: hasCompletedTopic);
        }

        var assessmentIds = assessments.Select(a => a.Id).ToHashSet();
        var attempts = await attemptRepository.GetAllAsync(
            filter: a => assessmentIds.Contains(a.AssessmentId),
            orderBy: q => q.OrderByDescending(a => a.StartedAt),
            ct: cancellationToken);

        var attemptsByAssessmentId = attempts.GroupBy(a => a.AssessmentId).ToDictionary(g => g.Key, g => g.ToList());

        var items = assessments
            .OrderBy(a => a.CreatedAt)
                    .Select(a =>
                    {
                        var assessmentAttempts = attemptsByAssessmentId.GetOrDefault(a.Id) ?? [];
                        var attemptDtos = assessmentAttempts
                            .Select(r => new AssessmentAttemptHistoryDto(
                                AttemptId: r.Id.ToString(),
                                StartedAt: r.StartedAt,
                                CompletedAt: r.CompletedAt,
                                Score: r.ScoredPoints == 0 && !r.CompletedAt.HasValue ? null : r.ScoredPoints))
                            .ToList();

                        var maxScore = assessmentAttempts.FirstOrDefault()?.MaxPoints ?? 0;
                        return new AssessmentHistoryItemDto(
                            AssessmentId: a.Id.ToString(),
                            Type: a.TestType,
                            MaxScore: maxScore,
                            Attempts: attemptDtos);
                    })
            .ToList();

        return new AssessmentHistoryDto(items, IsIntermediateAssessmentAvailable: hasCompletedTopic);
    }
}