using SkillMap.Business.RoadmapAssessments.Features.GetRoadmapAssessment;
using SkillMap.Business.RoadmapTest.Models;

namespace SkillMap.Business.RoadmapAssessments.Features.GetAssessment;

public record GetAttemptAssessmentContentQuery(long AttemptId) : ICommand<RoadmapAssessmentDto>;