using Newtonsoft.Json;
using SkillMap.Api.Base.Searching;

namespace SkillMap.Api.Roadmaps.Models;

public class PlainRoadmapsResponse : PaginationResponse
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