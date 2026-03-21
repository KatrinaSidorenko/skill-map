using Newtonsoft.Json;

using SkillMap.Core.Constants;

namespace SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

public class RoadmapSnapshot
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("items")]
    public List<LearningItemSnapshot> LearningItems { get; set; }
    [JsonProperty("connections")]
    public List<LearningItemsConnectionSnapshot> LearningItemsConnections { get; set; }
}

public record LearningItemSnapshot(string Id, string Title, string Description, LearningStatus Status);
public record LearningItemsConnectionSnapshot(string Id, string FromId, string ToId);