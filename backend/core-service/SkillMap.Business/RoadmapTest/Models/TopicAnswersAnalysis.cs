using LearningPlatform.RoadmapTests.Contracts;

namespace SkillMap.Business.RoadmapTest.Models;

public class TestAnswersAnalysis
{
    public Dictionary<string, TopicAnswersAnalysis> TopicsAnalysis { get; set; }
    public int TotalPossiblePoints => TopicsAnalysis.Values.Sum(t => t.TotalPossiblePoints);
    public int AchievedPoints => TopicsAnalysis.Values.Sum(t => t.AchievedPoints);
    public TestAnswersAnalysis(Dictionary<string, TopicAnswersAnalysis> topicsAnalysis)
    {
        TopicsAnalysis = topicsAnalysis;
    }
}
public class TopicAnswersAnalysis
{
    public Dictionary<string, QuestionAnalysisResult> QuestionsAnalysis { get; set; }
    public int TotalPossiblePoints => QuestionsAnalysis.Values.Sum(q => q.TotalPossiblePoints);
    public int AchievedPoints => QuestionsAnalysis.Values.Sum(q => q.AchievedPoints);
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
