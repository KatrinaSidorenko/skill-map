using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.Constants;

namespace SkillMap.Business.UserAccount.Features.GetUserDashboard;

[UsedImplicitly]
public class GetUserDashboardHandler(IRoadmapWorkspaceRepository workspaceRepository)
    : IRequestHandler<GetUserDashboardQuery, UserDashboardDto>
{
    private const int RecentTestsLimit = 10;

    public async Task<UserDashboardDto> Handle(GetUserDashboardQuery request, CancellationToken cancellationToken)
    {
        var workspaces = await workspaceRepository.GetUserWorkspacesWithAssessments(request.UserId, cancellationToken);

        var savedRoadmaps = workspaces.Count;

        var inProgressWorkspaces = new List<InProgressWorkspaceDto>();
        var activeCount = 0;

        foreach (var workspace in workspaces)
        {
            var totalItems = workspace.LearningItemProjections.Count;
            var completedItems = workspace.LearningItemProjections.Count(p =>
        p.Status == LearningStatus.Completed.ToStatusString());

            var (progress, status) = RoadmapWorkspaceSnapshotExtensions.CalculateSnapshotMetadata(totalItems, completedItems);

            if (status == LearningStatus.InProgress)
                activeCount++;

            inProgressWorkspaces.Add(new InProgressWorkspaceDto
            {
                WorkspaceId = workspace.Id.ToString(),
                Title = workspace.Title,
                Description = workspace.Description,
                ImageUrl = workspace.ImageUrl,
                Progress = progress,
                SavedAt = workspace.CreatedAt,
                Status = status.ToStatusString(),
                TotalNodes = totalItems,
            });
        }

        var allAttempts = workspaces
            .SelectMany(w => w.Assessments ?? [])
            .SelectMany(a => a.Attempts?.Select(attempt => (Assessment: a, Attempt: attempt)) ?? [])
            .OrderByDescending(x => x.Attempt.StartedAt)
            .ToList();

        var completedAttempts = allAttempts.Where(x => x.Attempt.CompletedAt.HasValue).ToList();

        var testsCompleted = completedAttempts.Count;
        var averageScore = testsCompleted > 0
            ? completedAttempts.Average(x => x.Attempt.MaxPoints > 0
            ? x.Attempt.ScoredPoints / x.Attempt.MaxPoints * 100
            : 0)
            : 0;

        var recentTests = allAttempts
            .Take(RecentTestsLimit)
           .Select(x => new RecentAssessmentAttemptDto
           {
               AttemptId = x.Attempt.Id.ToString(),
               AssessmentId = x.Assessment.Id.ToString(),
               Type = x.Assessment.TestType,
               Score = x.Attempt.CompletedAt.HasValue ? x.Attempt.ScoredPoints : null,
               MaxScore = x.Attempt.MaxPoints,
               StartedAt = x.Attempt.StartedAt,
               CompletedAt = x.Attempt.CompletedAt,
               Status = x.Attempt.CompletedAt.HasValue ? "completed" : "in_progress",
           })
           .ToList();

        return new UserDashboardDto
        {
            Stats = new DashboardStatsDto
            {
                SavedRoadmaps = savedRoadmaps,
                ActiveRoadmaps = activeCount,
                TestsCompleted = testsCompleted,
                AverageScore = Math.Round(averageScore, 1),
            },
            InProgressRoadmaps = inProgressWorkspaces,
            RecentTests = recentTests,
        };
    }
}
