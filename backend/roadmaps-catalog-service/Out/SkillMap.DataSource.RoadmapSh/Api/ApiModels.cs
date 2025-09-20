using Newtonsoft.Json;

namespace SkillMap.DataSource.RoadmapSh.Api;

public class RoadmapResponse
{
    [JsonProperty("nodes")]
    public List<RoadmapNode> Nodes { get; set; }

    [JsonProperty("edges")]
    public List<RoadmapEdge> Edges { get; set; }
}

public class RoadmapNode
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("data")]
    public RoadmapNodeData Data { get; set; }
}

public class RoadmapNodeData
{
    [JsonProperty("label")]
    public string Label { get; set; }
}

public class RoadmapEdge
{
    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("target")]
    public string Target { get; set; }
}

public class FileInfoResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }

    [JsonProperty("download_url")]
    public string DownloadUrl { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
    [JsonProperty("encoding")]
    public string Encoding { get; set; }
}