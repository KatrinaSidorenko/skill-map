using Newtonsoft.Json;

namespace SkillMap.Api.Models;

public class TreeResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("nodes")]
    public List<NodeResponse> Nodes { get; set; } = new List<NodeResponse>();
}

public class NodeResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("index")]
    public int Index { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("additionalProps")]
    public Dictionary<string, string> AdditionalProps { get; set; }
    [JsonProperty("parentId")]
    public string ParentId { get; set; }

    [JsonProperty("children")]
    public List<NodeResponse> Children { get; set; } = new List<NodeResponse>();
}