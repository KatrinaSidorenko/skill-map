using LearningPlatform.RoadmapTests.Contracts;

using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapTest.Models;

public class RoadmapTestConfigDto
{

    [JsonProperty("numberOfQuestions")]
    public int? NumberOfQuestions { get; set; } = 15;
    [JsonProperty("timeLimitInMinutes")]
    public int TimeLimitInMinutes { get; set; } = 15;
    [JsonProperty("difficultyLevel")]
    public string DifficultyLevel { get; set; } = Difficulty.Easy.ToDifficultyString();
}