using Newtonsoft.Json;

namespace SkillMap.Api.__old_Roadmap.UserRoadmap.Models;

public class UserRoadmapsResponse
{
    [JsonProperty("roadmaps")]
    public List<UserRoadmapResponse> Roadmaps { get; set; } = new List<UserRoadmapResponse>();
}

public class UserRoadmapResponse
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("roadmapId")]
    public string RoadmapId { get; set; }
}