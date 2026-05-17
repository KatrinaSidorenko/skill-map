using SkillMap.Core.Constants;

namespace SkillMap.Business.UserAccount.Features.GetUserDashboard;

public class UserDashboardDto
{
    public DashboardStatsDto Stats { get; init; }
    public List<InProgressWorkspaceDto> InProgressRoadmaps { get; init; }
    public List<RecentAssessmentAttemptDto> RecentTests { get; init; }
}

public class DashboardStatsDto
{
    public int SavedRoadmaps { get; init; }
  public int ActiveRoadmaps { get; init; }
    public int TestsCompleted { get; init; }
    public double AverageScore { get; init; }
}

public class InProgressWorkspaceDto
{
    public string WorkspaceId { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public double Progress { get; init; }
    public DateTime SavedAt { get; init; }
    public string Status { get; init; }
    public int TotalNodes { get; init; }
}

public class RecentAssessmentAttemptDto
{
    public string AttemptId { get; init; }
    public string AssessmentId { get; init; }
    public string Type { get; init; }
    public double? Score { get; init; }
    public double MaxScore { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string Status { get; init; }
}
