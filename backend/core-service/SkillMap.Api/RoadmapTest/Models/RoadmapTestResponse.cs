using Newtonsoft.Json;

namespace SkillMap.Api.RoadmapTest.Models;

public class RoadmapTestResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("questions")]
    public List<TopicQuestionResponse> Questions { get; set; }
}

public class TopicQuestionResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("answers")]
    public List<AnswerResponse> Answers { get; set; }
}

public class AnswerResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
}