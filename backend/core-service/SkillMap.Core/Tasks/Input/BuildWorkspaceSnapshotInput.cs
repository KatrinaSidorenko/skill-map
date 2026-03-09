using System.Text.Json.Serialization;

namespace SkillMap.Core.Tasks.Input;
public class BuildWorkspaceSnapshotInput
{
    [JsonPropertyName("workspaceId")]
    public long WorkspaceId { get; set; }
    [JsonPropertyName("roadmapId")]
    public string RoadmapId { get; set; }
    [JsonPropertyName("isInAuthorMode")]
    public bool IsInAuthorMode { get; set; }
}
