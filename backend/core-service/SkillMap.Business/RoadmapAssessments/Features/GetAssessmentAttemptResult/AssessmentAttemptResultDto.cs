using SkillMap.Core.RoadmapAssessments;

namespace SkillMap.Business.RoadmapAssessments.Features.GetAssessmentAttemptResult;

public record AssessmentAttemptResultDto(
    long AttemptId,
    string WorkspaceId,
    double TotalAchievedPoints,
    double TotalPossiblePoints,
    Dictionary<string, QuestionResultDto> QuestionResults);

public record QuestionResultDto(
    string QuestionId,
    string Text,
    string Type,
    bool IsCorrect,
    double AchievedPoints,
    double TotalPossiblePoints,
    Dictionary<string, AnswerDetailDto> AnswerDetails);

public record AnswerDetailDto(
    string AnswerId,
    string Text,
    bool IsCorrect,
    bool IsSelected);