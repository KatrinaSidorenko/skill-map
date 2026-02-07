using Newtonsoft.Json;

namespace SkillMap.Api.Roadmap.Models;

public class LearningItemMaterialResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
}