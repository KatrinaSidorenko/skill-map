using Newtonsoft.Json;

namespace SkillMap.Business.__old.ModifiedRoadmaps.Models;

public class TreePlainResponse
{
    [JsonProperty("nodes")]
    public List<PlainNodeResponse> Nodes { get; set; }
    [JsonProperty("edges")]
    public List<PlainEdgeResponse> Edges { get; set; }
}

public class PlainNodeResponse
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
}

public class PlainEdgeResponse
{
    [JsonProperty("source")]
    public string Source { get; set; }
    [JsonProperty("target")]
    public string Target { get; set; }
}

public class Tree
{
    [JsonProperty("nodes")]
    public List<NodeResponse> Nodes { get; set; }
    [JsonProperty("edges")]
    public List<(string, string)> Edges { get; set; }
}