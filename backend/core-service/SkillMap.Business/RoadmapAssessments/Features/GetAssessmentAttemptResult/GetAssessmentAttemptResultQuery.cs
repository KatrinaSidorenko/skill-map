namespace SkillMap.Business.RoadmapAssessments.Features.GetAssessmentAttemptResult;

public record GetAssessmentAttemptResultQuery(long AttemptId) : ICommand<AssessmentAttemptResultDto>;