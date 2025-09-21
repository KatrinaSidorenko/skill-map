using Newtonsoft.Json;

namespace SkillMap.Api.Models;

public class RoadmapsResponse
{
    [JsonProperty("roadmaps")]
    public List<RoadmapResponse> Roadmaps { get; set; } = new List<RoadmapResponse>();
}

public class RoadmapResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
}
