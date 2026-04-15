using Newtonsoft.Json;

using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.AddLearningItem;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.AddLearningItem;

public class AddLearningItemRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("baseVersion")]
    public int BaseVersion { get; set; }

    [JsonProperty("idempotencyKey")]
    public string IdempotencyKey { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }

    public AddLearningItemCommand ToCommand(long userRoadmapId)
        => new(userRoadmapId, Id, Title, Description, Status, Type, BaseVersion, IdempotencyKey);
}