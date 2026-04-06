namespace SkillMap.Business.RoadmapAssessments.Features.CreateAssessmentAttempt;

public record CreateAssessmentAttemptCommand(long AssessmentId, long UserId) : ICommand<long>;