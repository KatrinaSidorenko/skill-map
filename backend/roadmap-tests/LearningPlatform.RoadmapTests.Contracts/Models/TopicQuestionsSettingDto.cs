using Newtonsoft.Json;

namespace LearningPlatform.RoadmapTests.Contracts.Models;

public class TopicQuestionsSettingDto
{
    [JsonProperty("difficultyLevel")]
    public RoadmapTestDifficultyLevel DifficultyLevel { get; set; }
    [JsonProperty("questionsCount")]
    public int QuestionsCount { get; set; }

    [JsonProperty("types")]
    public List<TestQuestionType> Types { get; set; }
    [JsonIgnore]
    public List<string> TypeStrings =>
        Types.Select(t => t.ToQuestionTypeString()).ToList();
}
