using System.Text.Json.Serialization;

using SkillMap.Business.RoadmapAssessments.Features.GetAssessmentHistory;

namespace SkillMap.Api.RoadmapAssessments.GetAssessmentHistory;

public class AssessmentHistoryResponse
{
    [JsonPropertyName("items")]
    public List<AssessmentHistoryItemResponse> Items { get; init; }

    public static AssessmentHistoryResponse Create(AssessmentHistoryDto dto) => new()
    {
        Items = dto.Items.Select(AssessmentHistoryItemResponse.Create).ToList()
    };
}

public class AssessmentHistoryItemResponse
{
    [JsonPropertyName("testId")]
    public string TestId { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("maxScore")]
    public double MaxScore { get; init; }

    [JsonPropertyName("attempts")]
    public List<AssessmentAttemptHistoryResponse> Attempts { get; init; }

    public static AssessmentHistoryItemResponse Create(AssessmentHistoryItemDto dto) => new()
    {
        TestId = dto.AssessmentId,
        Type = dto.Type,
        MaxScore = dto.MaxScore,
        Attempts = dto.Attempts.Select(AssessmentAttemptHistoryResponse.Create).ToList()
    };
}

public class AssessmentAttemptHistoryResponse
{
    [JsonPropertyName("resultId")]
    public string ResultId { get; init; }

    [JsonPropertyName("startedAt")]
    public DateTime StartedAt { get; init; }

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; init; }

    [JsonPropertyName("score")]
    public double? Score { get; init; }

    public static AssessmentAttemptHistoryResponse Create(AssessmentAttemptHistoryDto dto) => new()
    {
        ResultId = dto.AttemptId,
        StartedAt = dto.StartedAt,
        CompletedAt = dto.CompletedAt,
        Score = dto.Score
    };
}