using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace SkillMap.Api.RoadmapsWorkspace.GetWorkspaceEventsStatus;

/// <summary>Request body: list of idempotency keys to look up.</summary>
public class GetWorkspaceEventsStatusRequest
{
    [JsonPropertyName("keys")]
    public List<string> IdempotencyKeys { get; set; } = [];
}
