using Newtonsoft.Json;

namespace SkillMap.Business.__old.ModifiedRoadmaps.Models;

public class LearningItem
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
}

public class LearningItemConnection : IEquatable<LearningItemConnection>
{
    [JsonProperty("sourceId")]
    public string SourceId { get; set; }
    [JsonProperty("targetId")]
    public string TargetId { get; set; }

    public override bool Equals(object obj)
        => Equals(obj as LearningItemConnection);

    public bool Equals(LearningItemConnection other)
    {
        return other != null &&
               SourceId == other.SourceId &&
               TargetId == other.TargetId;
    }

    public override int GetHashCode()
        => HashCode.Combine(SourceId, TargetId);
}