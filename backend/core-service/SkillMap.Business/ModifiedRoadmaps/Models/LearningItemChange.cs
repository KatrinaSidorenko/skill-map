using Newtonsoft.Json;

namespace SkillMap.Business.Roadmaps.Models;
public class LearningItemChange : IEquatable<LearningItemChange>
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public override bool Equals(object obj)
        => Equals(obj as LearningItemChange);

    public bool Equals(LearningItemChange other)
    {
        return other != null &&
               Id == other.Id &&
               Title == other.Title &&
               Description == other.Description &&
               Status == other.Status;
    }

    public override int GetHashCode()
        => HashCode.Combine(Id, Title, Description, Status);
}

public class DeleteLearningItemChange
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
}