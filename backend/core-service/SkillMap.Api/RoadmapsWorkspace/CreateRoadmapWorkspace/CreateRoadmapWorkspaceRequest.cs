using Newtonsoft.Json;

namespace SkillMap.Api.RoadmapsWorkspace.CreateRoadmapWorkspace;

public class CreateRoadmapWorkspaceRequest
{
    [JsonProperty("roadmapId")]
    public string RoadmapId { get; set; }
}
