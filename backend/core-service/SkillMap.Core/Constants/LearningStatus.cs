namespace SkillMap.Core.Constants;

public enum LearningStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Skipped = 3
}

public static class LearningStatusExtensions
{
    public static string ToStatusString(this LearningStatus status) => status switch
    {
        LearningStatus.NotStarted => LearningStatus.NotStarted.ToString().ToLower().ToLower(),
        LearningStatus.InProgress => LearningStatus.InProgress.ToString().ToLower(),
        LearningStatus.Completed => LearningStatus.Completed.ToString().ToLower(),
        LearningStatus.Skipped => LearningStatus.Skipped.ToString().ToLower(),
        _ => "unknown"
    };
}
