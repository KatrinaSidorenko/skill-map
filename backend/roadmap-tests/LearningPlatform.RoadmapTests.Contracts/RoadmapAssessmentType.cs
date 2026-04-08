namespace LearningPlatform.RoadmapTests.Contracts;

public enum RoadmapAssessmentType
{
    Initial = 0,
    Intermediate = 1,
}

public static class RoadmapTestTypeExtensions
{
    public static string ToFriendlyString(this RoadmapAssessmentType type) => type switch
    {
        RoadmapAssessmentType.Initial => "initial",
        RoadmapAssessmentType.Intermediate => "intermediate",
        _ => "Unknown"
    };
}