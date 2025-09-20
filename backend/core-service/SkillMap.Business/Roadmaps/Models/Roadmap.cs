using Newtonsoft.Json;

namespace SkillMap.Business.Roadmaps.Models;

public class Roadmap
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("progress")]
    public double Progress { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}
