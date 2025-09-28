using Newtonsoft.Json;

namespace SkillMap.Api.Roadmaps.Models;

public class PlainRoadmapsResponse
{
    [JsonProperty("roadmaps")]
    public List<PlainRoadmapResponse> Roadmaps { get; set; }
}

public class PlainRoadmapResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }
}