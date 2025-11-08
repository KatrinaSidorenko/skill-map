using LearningPlatform.RoadmapTests.Contracts.Models;
using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapTest.Models;

public class RoadmapTestDao
{
    public string RoadmapId { get; set; }
    public List<TopicQuestionsDto> Questions { get; set; }
    public Dictionary<string, TopicQuestionSetting> TopicSettings { get; set; }
}

public class RoadmapTestResult
{
    public List<QuestionResult> Questions { get; set; }
}

public class QuestionResult
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("topicId")]
    public string TopicId { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
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
