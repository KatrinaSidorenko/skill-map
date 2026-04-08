using Newtonsoft.Json;

using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItem;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.DeleteLearningItem;

public class DeleteLearningItemRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    [JsonProperty("idempotencyKey")]
    public string IdempotencyKey { get; set; }

    public DeleteLearningItemCommand ToCommand(long userRoadmapId)
        => new(userRoadmapId, Id, ClientWorkspaceVersion, IdempotencyKey);
}