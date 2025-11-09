using Newtonsoft.Json;
namespace SkillMap.Business.RoadmapTest.Models;

public class RoadmapTestAnswers
{
    [JsonProperty("questionAnswers")]
    public List<QuestionAnswer> QuestionAnswers { get; set; }
}

public class QuestionAnswer
{
    [JsonProperty("questionId")]
    public string QuestionId { get; set; }
}

public class SingleChoiceAnswer : QuestionAnswer
{
    [JsonProperty("selectedAnswerId")]
    public string SelectedAnswerId { get; set; }
}
