using LearningPlatform.Roadmap.Business.Contracts.Models;

using Newtonsoft.Json;

namespace SkillMap.Business.__old.ModifiedRoadmaps.Models;

// todo: what does saved mean?? bullshit name
public class SavedUerRoadmap
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }
    [JsonProperty("progress")]
    public double Progress { get; set; }
    [JsonProperty("savedAt")]
    public DateTime SavedAt { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("nodes")]
    public List<ModifiedNode> Nodes { get; set; }
    [JsonProperty("edges")]
    public List<Edge> Edges { get; set; }
}

public class ModifiedNode
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
}