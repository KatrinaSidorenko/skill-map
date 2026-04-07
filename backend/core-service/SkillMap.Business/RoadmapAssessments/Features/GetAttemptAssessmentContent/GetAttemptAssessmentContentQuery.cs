using SkillMap.Business.RoadmapAssessments.Features.GetRoadmapAssessment;

namespace SkillMap.Business.RoadmapAssessments.Features.GetAssessment;

public record GetAttemptAssessmentContentQuery(long AttemptId) : ICommand<RoadmapAssessmentDto>;