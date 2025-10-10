using Newtonsoft.Json;
using SkillMap.Api.Base.Searching;

namespace SkillMap.Api.Roadmaps.Models;

public class PlainRoadmapResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }
    [JsonProperty("savedAt")]
    public DateTime CreatedAt { get; set; }
}