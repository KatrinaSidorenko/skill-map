namespace SkillMap.Business.RoadmapAssessments.Features.CreateIntermediateAssessment;
public record CreateIntermediateAssessmentCommand(long WorkspaceId) : ICommand<long>;