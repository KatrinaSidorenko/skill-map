namespace SkillMap.Business.RoadmapTest.Models;

public class AnswersCheckResult
{
    public Dictionary<string, CheckedQuestion> QuestionResults { get; set; }
}

public class CheckedQuestion
{
    public string QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public int AchievedPoints { get; set; }
    public int TotalPossiblePoints { get; set; }
}

public class CheckedSingleAnswerQuestion : CheckedQuestion
{
    public string CorrectAnswerId { get; set; }
}
