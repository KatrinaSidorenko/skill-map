using Newtonsoft.Json;

namespace LearningPlatform.RoadmapTests.Contracts.Models;

public class TopicQuestionSetting
{
    [JsonProperty("difficultyLevel")]
    public RoadmapTestDifficultyLevel DifficultyLevel { get; set; }
    [JsonProperty("questionsCount")]
    public int QuestionsCount { get; set; }

    [JsonProperty("type")]
    public TestQuestionType Type { get; set; }
}
