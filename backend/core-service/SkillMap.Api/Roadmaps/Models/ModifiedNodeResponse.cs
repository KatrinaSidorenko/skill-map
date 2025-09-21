using Newtonsoft.Json;
using SkillMap.Business.Roadmaps.Models;

namespace SkillMap.Api.Roadmaps.Models;

public class ModifiedNodeResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("progress")]
    public double Progress { get; set; }
    [JsonProperty("parentId")]
    public string ParentId { get; set; }
    [JsonProperty("additionalProps")]
    public Dictionary<string, object> AdditinalProps { get; set; }
    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("children")]
    public List<CustomizedUserRoadmapLearningItem> Children { get; set; }
}