namespace LearningPlatform.RoadmapTests.Contracts;

public enum RoadmapTestType
{
    Initial = 0,
    Intermediate = 1,
}

public static class RoadmapTestTypeExtensions
{
    public static string ToFriendlyString(this RoadmapTestType type) => type switch
    {
        RoadmapTestType.Initial => "initial",
        RoadmapTestType.Intermediate => "intermediate",
        _ => "Unknown"
    };
}