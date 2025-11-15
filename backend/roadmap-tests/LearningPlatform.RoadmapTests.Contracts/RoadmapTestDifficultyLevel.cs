namespace LearningPlatform.RoadmapTests.Contracts;

public enum RoadmapTestDifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public static class RoadmapTestDifficultyLevelExtensions
{
    public static string ToDifficultyString(this RoadmapTestDifficultyLevel level)
    {
        return level switch
        {
            RoadmapTestDifficultyLevel.Easy => "easy",
            RoadmapTestDifficultyLevel.Medium => "medium",
            RoadmapTestDifficultyLevel.Hard => "hard",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }

    public static RoadmapTestDifficultyLevel FromDifficultyString(this string level)
    {
        return level.ToLower().ToLower() switch
        {
            "easy" => RoadmapTestDifficultyLevel.Easy,
            "medium" => RoadmapTestDifficultyLevel.Medium,
            "hard" => RoadmapTestDifficultyLevel.Hard,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }
}
