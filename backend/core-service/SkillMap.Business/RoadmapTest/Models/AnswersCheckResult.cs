using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SkillMap.Business.RoadmapTest.Models;

public class AnswersCheckResult
{
    [JsonProperty("questionResults")]
    public Dictionary<string, CheckedQuestion> QuestionResults { get; set; }
}

public class CheckedQuestion
{
    [JsonProperty("questionId")]
    public string QuestionId { get; set; }
    [JsonProperty("isCorrect")]
    public bool IsCorrect { get; set; }
    [JsonProperty("achievedPoints")]
    public int AchievedPoints { get; set; }
    [JsonProperty("totalPossiblePoints")]
    public int TotalPossiblePoints { get; set; }
    [JsonProperty("questionType")]
    public string QuestionType { get; set; }
}

public class CheckedSingleAnswerQuestion : CheckedQuestion
{
    [JsonProperty("correctAnswerId")]
    public string CorrectAnswerId { get; set; }
}
