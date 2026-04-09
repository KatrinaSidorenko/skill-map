using LearningPlatform.Shared.Api;

namespace SkillMap.Api.RoadmapAssessments;

internal static class RoadmapAssessmentsApiPaths
{
    private const string AssessmentsRoot = $"{ApiPaths.Root}/assessments";

    public const string CreateInitialAssessment    = $"{AssessmentsRoot}/{{workspaceId}}/initial";
    public const string CreateIntermediateAssessment  = $"{AssessmentsRoot}/{{workspaceId}}/intermediate";
    public const string CreateAssessmentAttempt       = $"{AssessmentsRoot}/{{assessmentId}}/attempts";

    public const string GetAttemptAssessmentContent= $"{AssessmentsRoot}/attempts/{{attemptId}}";
    public const string EvaluateAssessment= $"{AssessmentsRoot}/attempts/{{attemptId}}/evaluate";
    public const string GetAssessmentAttemptResult    = $"{AssessmentsRoot}/attempts/{{attemptId}}/result";
    public const string GetRoadmapStateSuggestions    = $"{AssessmentsRoot}/attempts/{{attemptId}}/suggestions";
    public const string ApplyRoadmapStateSuggestions  = $"{AssessmentsRoot}/attempts/{{attemptId}}/suggestions/apply";

    public const string GetAssessmentHistory          = $"{AssessmentsRoot}/workspace/{{workspaceId}}/history";
}