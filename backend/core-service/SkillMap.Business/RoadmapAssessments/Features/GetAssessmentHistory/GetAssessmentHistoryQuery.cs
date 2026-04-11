namespace SkillMap.Business.RoadmapAssessments.Features.GetAssessmentHistory;

public record GetAssessmentHistoryQuery(long WorkspaceId) : ICommand<AssessmentHistoryDto>;

public record AssessmentHistoryDto(List<AssessmentHistoryItemDto> Items, bool IsIntermediateAssessmentAvailable);

public record AssessmentHistoryItemDto(
    string AssessmentId,
    string Type,
    double MaxScore,
    List<AssessmentAttemptHistoryDto> Attempts);

public record AssessmentAttemptHistoryDto(
    string AttemptId,
    DateTime StartedAt,
    DateTime? CompletedAt,
    double? Score);