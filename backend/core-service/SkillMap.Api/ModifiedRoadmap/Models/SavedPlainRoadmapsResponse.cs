using Newtonsoft.Json;

using SkillMap.Api.Roadmaps.Models;

namespace SkillMap.Api.ModifiedRoadmap.Models;

public class SavedPlainRoadmapResponse : PlainRoadmapResponse
{
    [JsonProperty("progress")]
    public double Progress { get; set; }
    [JsonProperty("savedAt")]
    public DateTime SavedAt { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
}