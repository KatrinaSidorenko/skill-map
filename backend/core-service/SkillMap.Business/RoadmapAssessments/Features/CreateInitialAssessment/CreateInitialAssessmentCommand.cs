namespace SkillMap.Business.RoadmapAssessments.Features.CreateInitialAssessment;

public record CreateInitialAssessmentCommand(long WorkspaceId) : ICommand<long>;
