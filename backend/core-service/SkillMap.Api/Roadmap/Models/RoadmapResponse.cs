using LearningPlatform.Roadmap.Business.Contracts.Models;
using Newtonsoft.Json;

namespace SkillMap.Api.Roadmaps.Models;

public class RoadmapResponse
{
    [JsonProperty("roadmap")]
    public RoadmapDto Roadmap { get; set; }
}

