using Newtonsoft.Json;

namespace SkillMap.Api.RoadmapTest.Models;

public class TestResultResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    public TestResultResponse(string testResultId)
    {
        Id = testResultId;
    }
}