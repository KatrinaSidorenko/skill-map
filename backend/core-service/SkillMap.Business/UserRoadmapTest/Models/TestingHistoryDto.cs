using Newtonsoft.Json;

namespace SkillMap.Business.UserRoadmapTest.Models;

public class TestingHistoryDto
{
    [JsonProperty("items")]
    public List<TestHistoryItemDto> Items { get; set; } = new();
}

public class TestHistoryItemDto
{
    [JsonProperty("testId")]
    public string TestId { get; set; }
    [JsonProperty("maxScore")]
    public double MaxScore { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("attempts")]
    public List<TestAttemptDto> Attempts { get; set; } = new();
}

public class TestAttemptDto
{
    [JsonProperty("resultId")]
    public string ResultId { get; set; }
    [JsonProperty("startedAt")]
    public DateTime StartedAt { get; set; }
    [JsonProperty("completedAt")]
    public DateTime? CompletedAt { get; set; }
    [JsonProperty("score")]
    public double? Score { get; set; }
}
