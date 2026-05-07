using Newtonsoft.Json;

namespace SkillMap.Api.RoadmapsWorkspace.CreateEmptyRoadmapWorkspace;

public class CreateEmptyRoadmapWorkspaceRequest
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("imageUrl")]
    public string? ImageUrl { get; set; }
}
