using Newtonsoft.Json;

namespace SkillMap.Api.Roadmaps.Models;

public class RoadmapDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("nodes")]
    public List<Node> Nodes { get; set; }
    [JsonProperty("edges")]
    public List<Edge> Edges { get; set; }
}

public class Node
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
}

public class Edge
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("source")]
    public string? Source { get; set; }
    [JsonProperty("target")]
    public string? Target { get; set; }
}
