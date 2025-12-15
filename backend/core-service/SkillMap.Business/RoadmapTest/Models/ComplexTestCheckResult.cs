using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapTest.Models;

public class ComplexTestCheckResult
{
    [JsonProperty("questionResults")]
    public Dictionary<string, TestQuestionResult> QuestionResults { get; set; }
    [JsonProperty("totalAchievedPoints")]
    public double TotalAchievedPoints => QuestionResults.Values.Sum(q => q.AchievedPoints);
    [JsonProperty("totalPossiblePoints")]
    public double TotalPossiblePoints => QuestionResults.Values.Sum(q => q.TotalPossiblePoints);
}

public class TestQuestionResult
{
    [JsonProperty("questionId")]
    public string QuestionId { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("isCorrect")]
    public bool IsCorrect { get; set; }
    [JsonProperty("achievedPoints")]
    public double AchievedPoints { get; set; }
    [JsonProperty("totalPossiblePoints")]
    public double TotalPossiblePoints { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("answerDetails")]
    public Dictionary<string, AnswerDetail> AnswerDetails { get; set; }

}

public class AnswerDetail
{
    [JsonProperty("answerId")]
    public string AnswerId { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("isCorrect")]
    public bool IsCorrect { get; set; }
    [JsonProperty("isSelected")]
    public bool IsSelected { get; set; }
}

public class SingleChoiceAnswerDetail : AnswerDetail
{
    
}
