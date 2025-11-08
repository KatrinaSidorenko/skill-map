using Newtonsoft.Json;

namespace LearningPlatform.RoadmapTests.Contracts.Models;

public record Topic(string Id, string Name, string Description);
public class TopicQuestionsDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("questions")]
    public List<QuestionDto> Questions { get; set; }
}
public class QuestionDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("answers")]
    public List<AnswerDto> Answers { get; set; }
}

public class AnswerDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("isCorrect")]
    public bool IsCorrect { get; set; }
}
