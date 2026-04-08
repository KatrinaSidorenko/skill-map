using SkillMap.Api.RoadmapAssessments.ApplyRoadmapStateSuggestions;
using SkillMap.Api.RoadmapAssessments.CreateAssessmentAttempt;
using SkillMap.Api.RoadmapAssessments.CreateIntermediateAssessment;
using SkillMap.Api.RoadmapAssessments.EvaluateRoadmapAssessment;
using SkillMap.Api.RoadmapAssessments.GetAssessment;
using SkillMap.Api.RoadmapAssessments.GetAssessmentAttemptResult;
using SkillMap.Api.RoadmapAssessments.GetAssessmentHistory;
using SkillMap.Api.RoadmapAssessments.GetRoadmapStateSuggestions;

namespace SkillMap.Api.RoadmapAssessments;

internal static class RoadmapAssessmentsEndpoints
{
    internal static void MapRoadmapAssessments(this WebApplication app)
    {
        app.MapCreateIntermediateAssessment();
        app.MapGetAttemptAssessment();
        app.MapEvaluateRoadmapAssessment();
        app.MapCreateAssessmentAttempt();
        app.MapGetAssessmentAttemptResult();
        app.MapGetAssessmentHistory();
        app.MapGetRoadmapStateSuggestions();
        app.MapApplyRoadmapStateSuggestions();
    }
}