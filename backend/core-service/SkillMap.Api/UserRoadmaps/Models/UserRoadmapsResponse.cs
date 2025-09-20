using Newtonsoft.Json;

namespace SkillMap.Api.UserRoadmaps.Models;

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
