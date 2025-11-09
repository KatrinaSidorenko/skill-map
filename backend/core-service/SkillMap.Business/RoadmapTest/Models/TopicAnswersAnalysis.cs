using LearningPlatform.RoadmapTests.Contracts;

namespace SkillMap.Business.RoadmapTest.Models;

public class TopicAnswersAnalysis
{
    public Dictionary<string, QuestionAnalysisResult> QuestionsAnalysis { get; set; }
}

public class QuestionAnalysisResult
{
    public int TotalPossiblePoints { get; set; }
    public int AchievedPoints { get; set; }
    public TestQuestionType QuestionType { get; set; }
}

public class SingleAnswerQuestionAnalysisResult : QuestionAnalysisResult
{
    public string SelectedAnswerId { get; set; }
    public string CorrectAnswerId { get; set; }
    public bool IsCorrect => AchievedPoints == TotalPossiblePoints;
}
