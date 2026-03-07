using Newtonsoft.Json;

namespace SkillMap.Api.Roadmaps.Models;

public class PlainRoadmapResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }
    [JsonProperty("savedAt")]
    public DateTime CreatedAt { get; set; }
}