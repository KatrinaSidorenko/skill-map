using Newtonsoft.Json;

using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.UpdateLearningItem;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.UpdateLearningItem;

public class UpdateLearningItemRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    [JsonProperty("idempotencyKey")]
    public string IdempotencyKey { get; set; }

    public UpdateLearningItemCommand ToCommand(long userRoadmapId)
        => new(userRoadmapId, Id, Title, Description, Status, ClientWorkspaceVersion, IdempotencyKey);
}
