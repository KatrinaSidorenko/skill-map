namespace SkillMap.Core.Constants;

public enum LearningStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Skip = 3,
    Repeat = 4,
    Upcoming = 5
}

public static class LearningStatusExtensions
{
    public static string ToStatusString(this LearningStatus status) => status switch
    {
        LearningStatus.NotStarted => LearningStatus.NotStarted.ToString().ToLower().ToLower(),
        LearningStatus.InProgress => LearningStatus.InProgress.ToString().ToLower(),
        LearningStatus.Completed => LearningStatus.Completed.ToString().ToLower(),
        LearningStatus.Skip => LearningStatus.Skip.ToString().ToLower(),
        LearningStatus.Repeat => LearningStatus.Repeat.ToString().ToLower(),
        LearningStatus.Upcoming => LearningStatus.Upcoming.ToString().ToLower(),
        _ => "unknown"
    };

    public static LearningStatus? FromStatusStringOrDefault(this string status)
    {
        return status.ToLower() switch
        {
            "notstarted" => LearningStatus.NotStarted,
            "inprogress" => LearningStatus.InProgress,
            "completed" => LearningStatus.Completed,
            "skip" => LearningStatus.Skip,
            "repeat" => LearningStatus.Repeat,
            "upcoming" => LearningStatus.Upcoming,
            _ => null
        };
    }

    public static List<string> GetStatuses()
    {
        return Enum.GetValues(typeof(LearningStatus))
            .Cast<LearningStatus>()
            .Select(s => s.ToStatusString())
            .ToList();
    }
}