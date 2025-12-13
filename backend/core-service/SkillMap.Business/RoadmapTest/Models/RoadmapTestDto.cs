using LearningPlatform.RoadmapTests.Contracts.Models;
using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapTest.Models;

public class RoadmapTestDao
{
    public string Id { get; set; }
    public string RoadmapId { get; set; }
    public string UserRoadmapId { get; set; }
    public string Type { get; set; }
    public List<TopicQuestionsDto> TopicQuestions { get; set; }
    public Dictionary<string, TopicQuestionsSettingDto> TopicSettings { get; set; }
    public RoadmapTestConfigDto TestConfig { get; set; }
}

public class RoadmapTestResultDto
{
    [JsonProperty("testId")]
    public string TestId { get; set; }
    [JsonProperty("questions")]
    public List<QuestionResultDto> Questions { get; set; }
}

public class QuestionResultDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("topicId")]
    public string TopicId { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("answers")]
    public List<AnswerResult> Answers { get; set; }
}

public class AnswerResult
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
}
