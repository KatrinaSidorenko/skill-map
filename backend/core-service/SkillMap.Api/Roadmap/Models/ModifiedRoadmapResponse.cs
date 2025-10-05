using Newtonsoft.Json;
using SkillMap.Business.Roadmaps.Models;

namespace SkillMap.Api.Roadmaps.Models;

public class ModifiedRoadmapResponse
{
    //[JsonProperty("rootNode")]
    //public RoadmapDto Roadmap { get; set; }

    [JsonProperty("nodes")]
    public List<ModifiedNodeResponse> Nodes { get; set; } = new List<ModifiedNodeResponse>();
}
