using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkillMap.Core.Entities.UserRoadmapTest;

public class RoadmapTestResult
{
    public Dictionary<string, TopicAnswersAnalysis> TopicsAnalysis { get; set; }

    [JsonIgnore]
    public int TotalPossiblePoints => TopicsAnalysis.Values.Sum(t => t.TotalPossiblePoints);
    [JsonIgnore]
    public int AchievedPoints => TopicsAnalysis.Values.Sum(t => t.AchievedPoints);
}

public class TopicAnswersAnalysis
{
    public Dictionary<string, QuestionAnalysisResult> QuestionsAnalysis { get; set; }
    [JsonIgnore]
    public int TotalPossiblePoints => QuestionsAnalysis.Values.Sum(q => q.TotalPossiblePoints);
    [JsonIgnore]
    public int AchievedPoints => QuestionsAnalysis.Values.Sum(q => q.AchievedPoints);
}

public class QuestionAnalysisResult
{
    public int TotalPossiblePoints { get; set; }
    public int AchievedPoints { get; set; }
    public string QuestionType { get; set; }
    public string SelectedAnswerId { get; set; }
    public string CorrectAnswerId { get; set; }
    [JsonIgnore]
    public bool IsCorrect => AchievedPoints == TotalPossiblePoints;
}

public class SingleAnswerQuestionAnalysisResultDto : QuestionAnalysisResult
{

}
