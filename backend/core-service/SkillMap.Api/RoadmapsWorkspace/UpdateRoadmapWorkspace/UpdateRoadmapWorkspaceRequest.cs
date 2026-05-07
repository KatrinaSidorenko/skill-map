using System.Text.Json.Serialization;

namespace SkillMap.Api.RoadmapsWorkspace.UpdateRoadmapWorkspace;

public class UpdateRoadmapWorkspaceRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }
}
