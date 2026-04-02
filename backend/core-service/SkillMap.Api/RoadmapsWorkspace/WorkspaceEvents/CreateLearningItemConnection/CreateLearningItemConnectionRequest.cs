using Newtonsoft.Json;

using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.CreateLearningItemConnection;

public class CreateLearningItemConnectionRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("target")]
    public string Target { get; set; }

    [JsonProperty("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    [JsonProperty("idempotencyKey")]
    public string IdempotencyKey { get; set; }

    public CreateLearningItemConnectionCommand ToCommand(long userRoadmapId)
        => new(userRoadmapId, Id, Source, Target, ClientWorkspaceVersion, IdempotencyKey);
}
