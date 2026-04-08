using System.Text.Json.Serialization;

using SkillMap.Business.RoadmapAssessments.Features.GetAssessmentAttemptResult;

namespace SkillMap.Api.RoadmapAssessments.GetAssessmentAttemptResult;

public class AssessmentAttemptResultResponse
{
    [JsonPropertyName("attemptId")]
    public long AttemptId { get; init; }

    [JsonPropertyName("workspaceId")]
    public string WorkspaceId { get; init; }

    [JsonPropertyName("totalAchievedPoints")]
    public double TotalAchievedPoints { get; init; }

    [JsonPropertyName("totalPossiblePoints")]
    public double TotalPossiblePoints { get; init; }

    [JsonPropertyName("questionResults")]
    public Dictionary<string, QuestionResultResponse> QuestionResults { get; init; }

    public static AssessmentAttemptResultResponse Create(AssessmentAttemptResultDto dto) => new()
    {
        AttemptId = dto.AttemptId,
        WorkspaceId = dto.WorkspaceId,
        TotalAchievedPoints = dto.TotalAchievedPoints,
        TotalPossiblePoints = dto.TotalPossiblePoints,
        QuestionResults = dto.QuestionResults.ToDictionary(
            kvp => kvp.Key,
            kvp => QuestionResultResponse.Create(kvp.Value)),
    };
}

public class QuestionResultResponse
{
    [JsonPropertyName("questionId")]
    public string QuestionId { get; init; }

    [JsonPropertyName("text")]
    public string Text { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("isCorrect")]
    public bool IsCorrect { get; init; }

    [JsonPropertyName("achievedPoints")]
    public double AchievedPoints { get; init; }

    [JsonPropertyName("totalPossiblePoints")]
    public double TotalPossiblePoints { get; init; }

    [JsonPropertyName("answerDetails")]
    public Dictionary<string, AnswerDetailResponse> AnswerDetails { get; init; }

    public static QuestionResultResponse Create(QuestionResultDto dto) => new()
    {
        QuestionId = dto.QuestionId,
        Text = dto.Text,
        Type = dto.Type,
        IsCorrect = dto.IsCorrect,
        AchievedPoints = dto.AchievedPoints,
        TotalPossiblePoints = dto.TotalPossiblePoints,
        AnswerDetails = dto.AnswerDetails.ToDictionary(
  kvp => kvp.Key,
  kvp => AnswerDetailResponse.Create(kvp.Value)),
    };
}

public class AnswerDetailResponse
{
    [JsonPropertyName("answerId")]
    public string AnswerId { get; init; }

    [JsonPropertyName("text")]
    public string Text { get; init; }

    [JsonPropertyName("isCorrect")]
    public bool IsCorrect { get; init; }

    [JsonPropertyName("isSelected")]
    public bool IsSelected { get; init; }

    public static AnswerDetailResponse Create(AnswerDetailDto dto) => new()
    {
        AnswerId = dto.AnswerId,
        Text = dto.Text,
        IsCorrect = dto.IsCorrect,
        IsSelected = dto.IsSelected,
    };
}