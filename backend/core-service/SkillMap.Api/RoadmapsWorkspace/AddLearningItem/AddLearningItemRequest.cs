using Newtonsoft.Json;

using SkillMap.Business.PersonalizedRoadmaps.Features.AddLearningItem;

namespace SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;

public class AddLearningItemRequest
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    public AddLearningItemCommand ToCommand(long userRoadmapId)
        => new (userRoadmapId, Title, Description, Status, ClientWorkspaceVersion);
}

