using LearningPlatform.RoadmapTests.Contracts;

namespace SkillMap.Business.RoadmapTest.Models;

public class RoadmapTestResultsDto
{
    public Dictionary<string, TopicAnswersAnalysisDto> TopicsAnalysis { get; set; }
    public int TotalPossiblePoints => TopicsAnalysis.Values.Sum(t => t.TotalPossiblePoints);
    public int AchievedPoints => TopicsAnalysis.Values.Sum(t => t.AchievedPoints);
    public RoadmapTestResultsDto(Dictionary<string, TopicAnswersAnalysisDto> topicsAnalysis)
    {
        TopicsAnalysis = topicsAnalysis;
    }
}
public class TopicAnswersAnalysisDto
{
    public Dictionary<string, QuestionAnalysisResultDto> QuestionsAnalysis { get; set; }
    public int TotalPossiblePoints => QuestionsAnalysis.Values.Sum(q => q.TotalPossiblePoints);
    public int AchievedPoints => QuestionsAnalysis.Values.Sum(q => q.AchievedPoints);

    public TopicAnswersAnalysisDto(Dictionary<string, QuestionAnalysisResultDto> questionsAnalysis)
    {
        QuestionsAnalysis = questionsAnalysis;
    }
}

public class QuestionAnalysisResultDto
{
    public int TotalPossiblePoints { get; set; }
    public int AchievedPoints { get; set; }
    public TestQuestionType QuestionType { get; set; }
    public string SelectedAnswerId { get; set; }
    public string CorrectAnswerId { get; set; }
    public bool IsCorrect => AchievedPoints == TotalPossiblePoints;
}

public class SingleAnswerQuestionAnalysisResultDto : QuestionAnalysisResultDto { }