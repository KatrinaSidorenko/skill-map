using Newtonsoft.Json;
using SkillMap.Business.Roadmaps.Models;

namespace SkillMap.Infrastructure.Roadmaps.Models;

public class RoadmapsResponse
{
    [JsonProperty("roadmaps")]
    public List<RoadmapDto> Roadmaps { get; set; }
}
