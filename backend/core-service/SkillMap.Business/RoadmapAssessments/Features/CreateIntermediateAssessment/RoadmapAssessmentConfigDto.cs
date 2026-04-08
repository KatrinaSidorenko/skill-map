using LearningPlatform.RoadmapTests.Contracts;

using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapAssessments.Features.CreateIntermediateAssessment;
internal class RoadmapAssessmentConfigDto
{
    [JsonProperty("numberOfQuestions")]
    public int? NumberOfQuestions { get; set; } = 15;
    [JsonProperty("timeLimitInMinutes")]
    public int TimeLimitInMinutes { get; set; } = 15;
    [JsonProperty("difficultyLevel")]
    public string DifficultyLevel { get; set; } = Difficulty.Easy.ToDifficultyString();
}
