namespace SkillMap.Business.RoadmapAssessments.Features.EvaluateRoadmapAssessment;

public record EvaluateRoadmapAssessmentCommand(long AttemptId, List<ProvidedAnswer> ProvidedAnswers) : ICommand<long>;

public record ProvidedAnswer(string QuestionId, string Type, string? SelectedAnswerId);