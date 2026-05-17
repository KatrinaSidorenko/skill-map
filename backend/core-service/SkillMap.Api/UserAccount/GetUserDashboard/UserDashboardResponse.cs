using System.Text.Json.Serialization;

using SkillMap.Business.UserAccount.Features.GetUserDashboard;

namespace SkillMap.Api.UserAccount.GetUserDashboard;

public class UserDashboardResponse
{
    [JsonPropertyName("stats")]
    public DashboardStatsResponse Stats { get; init; }

    [JsonPropertyName("inProgressRoadmaps")]
    public List<InProgressWorkspaceResponse> InProgressRoadmaps { get; init; }

    [JsonPropertyName("recentTests")]
    public List<RecentAssessmentAttemptResponse> RecentTests { get; init; }

    public static UserDashboardResponse Create(UserDashboardDto dto) => new()
    {
        Stats = DashboardStatsResponse.Create(dto.Stats),
        InProgressRoadmaps = dto.InProgressRoadmaps.Select(InProgressWorkspaceResponse.Create).ToList(),
        RecentTests = dto.RecentTests.Select(RecentAssessmentAttemptResponse.Create).ToList(),
    };
}

public class DashboardStatsResponse
{
    [JsonPropertyName("savedRoadmaps")]
    public int SavedRoadmaps { get; init; }

    [JsonPropertyName("activeRoadmaps")]
    public int ActiveRoadmaps { get; init; }

    [JsonPropertyName("testsCompleted")]
    public int TestsCompleted { get; init; }

    [JsonPropertyName("averageScore")]
    public double AverageScore { get; init; }

    public static DashboardStatsResponse Create(DashboardStatsDto dto) => new()
    {
        SavedRoadmaps = dto.SavedRoadmaps,
        ActiveRoadmaps = dto.ActiveRoadmaps,
        TestsCompleted = dto.TestsCompleted,
        AverageScore = dto.AverageScore,
    };
}

public class InProgressWorkspaceResponse
{
    [JsonPropertyName("workspaceId")]
    public string WorkspaceId { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; init; }

    [JsonPropertyName("progress")]
    public double Progress { get; init; }

    [JsonPropertyName("savedAt")]
    public DateTime SavedAt { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; }

    [JsonPropertyName("totalNodes")]
    public int TotalNodes { get; init; }

    public static InProgressWorkspaceResponse Create(InProgressWorkspaceDto dto) => new()
    {
        WorkspaceId = dto.WorkspaceId,
        Title = dto.Title,
        Description = dto.Description,
        ImageUrl = dto.ImageUrl,
        Progress = dto.Progress,
        SavedAt = dto.SavedAt,
        Status = dto.Status,
        TotalNodes = dto.TotalNodes,
    };
}

public class RecentAssessmentAttemptResponse
{
    [JsonPropertyName("attemptId")]
    public string AttemptId { get; init; }

    [JsonPropertyName("assessmentId")]
    public string AssessmentId { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("score")]
    public double? Score { get; init; }

    [JsonPropertyName("maxScore")]
    public double MaxScore { get; init; }

    [JsonPropertyName("startedAt")]
    public DateTime StartedAt { get; init; }

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; }

    public static RecentAssessmentAttemptResponse Create(RecentAssessmentAttemptDto dto) => new()
    {
        AttemptId = dto.AttemptId,
        AssessmentId = dto.AssessmentId,
        Type = dto.Type,
        Score = dto.Score,
        MaxScore = dto.MaxScore,
        StartedAt = dto.StartedAt,
        CompletedAt = dto.CompletedAt,
        Status = dto.Status,
    };
}
