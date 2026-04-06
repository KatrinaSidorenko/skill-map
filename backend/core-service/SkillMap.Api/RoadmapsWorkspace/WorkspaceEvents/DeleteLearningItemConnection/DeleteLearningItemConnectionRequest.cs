using Newtonsoft.Json;

using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItemConnection;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.DeleteLearningItemConnection;

public class DeleteLearningItemConnectionRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    [JsonProperty("idempotencyKey")]
    public string IdempotencyKey { get; set; }

    public DeleteLearningItemConnectionCommand ToCommand(long userRoadmapId)
        => new(userRoadmapId, Id, ClientWorkspaceVersion, IdempotencyKey);
}