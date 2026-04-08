using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace SkillMap.Core.Tasks.Input;
public class BuildWorkspaceSnapshotInput
{
    [JsonProperty("workspaceId")]
    public long WorkspaceId { get; set; }
    [JsonProperty("roadmapId")]
    public string RoadmapId { get; set; }
    [JsonProperty("isInAuthorMode")]
    public bool IsInAuthorMode { get; set; }
}