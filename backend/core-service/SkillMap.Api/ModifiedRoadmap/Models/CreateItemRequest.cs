using Newtonsoft.Json;

namespace SkillMap.Api.ModifiedRoadmap.Models;

public class CreateNodeRequest
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
}

public class CreateEdgeRequest
{
    [JsonProperty("sourceId")]
    public string SourceId { get; set; }
    [JsonProperty("targetId")]
    public string TargetId { get; set; }
}
