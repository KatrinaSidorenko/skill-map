using Newtonsoft.Json;

using SkillMap.Business.PersonalizedRoadmaps.AddLearningItem;

namespace SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;

public class AddLearningItemRequest
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }

    public AddLearningItemCommand ToCommand(long userRoadmapId)
        => new AddLearningItemCommand(userRoadmapId, Title, Description ?? string.Empty, Status);
}

