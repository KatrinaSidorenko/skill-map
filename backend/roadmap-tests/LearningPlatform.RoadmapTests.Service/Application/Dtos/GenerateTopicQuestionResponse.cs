using LearningPlatform.RoadmapTests.Contracts;
using Newtonsoft.Json;

namespace LearningPlatform.RoadmapTests.Service.Application.Models;

public sealed class TopicQuestionsDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("questions")]
    public List<QuestionDto> Questions { get; set; } = new();
}

public sealed class QuestionDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("type")]
    public TestQuestionType Type { get; set; }
    [JsonProperty("answers")]
    public List<AnswerDto> Answers { get; set; } = new();
    [JsonIgnore]
    public bool IsGenerated { get; set; }
}

public sealed class AnswerDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("isCorrect")]
    public bool IsCorrect { get; set; }
}
