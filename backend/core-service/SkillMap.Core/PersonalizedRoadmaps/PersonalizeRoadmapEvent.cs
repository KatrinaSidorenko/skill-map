using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapBookmarks;

namespace SkillMap.Core.PersonalizedRoadmaps;

public partial class RuleName
{
    private const string Prefix = "PRE_";
    public const string PersonalizeRoadmapEventMetadataCannotBeNull = $"{Prefix}001";
}
public class PersonalizeRoadmapEvent : TrackedEntity
{
    public long UserRoadmapId { get; set; }
    public EventType EventType { get; set; }
    public string? Metadata { get; set; }
    public int Version { get; set; } // can we do it self-incremented in db?

    public virtual RoadmapBookmark UserRoadmap { get; set; }
    public PersonalizeRoadmapEvent() { }
    public PersonalizeRoadmapEvent(long userRoadmapId, EventType eventType, object metadata)
    {
        if (metadata == null)
        {
            throw new BusinessRuleException(RuleName.PersonalizeRoadmapEventMetadataCannotBeNull);
        }

        UserRoadmapId = userRoadmapId;
        EventType = eventType;
        Metadata = System.Text.Json.JsonSerializer.Serialize(metadata);
    }
}