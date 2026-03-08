using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Core.PersonalizedRoadmaps;

public partial class RuleName
{
    private const string Prefix = "PRE_";
    public const string PersonalizeRoadmapEventMetadataCannotBeNull = $"{Prefix}001";
}
public class RoadmapWorkspaceEvent : TrackedEntity
{
    public long RoadmapWorkspaceId { get; private set; }
    public EventType EventType { get; private set; }
    public string? Metadata { get; private set; }
    public int Version { get; private set; } // can we do it self-incremented in db?

    public virtual RoadmapWorkspace RoadmapFork { get; set; }
    public RoadmapWorkspaceEvent() { }
    public RoadmapWorkspaceEvent(long userRoadmapId, EventType eventType, object metadata)
    {
        if (metadata == null)
        {
            throw new BusinessRuleException(RuleName.PersonalizeRoadmapEventMetadataCannotBeNull);
        }

        RoadmapWorkspaceId = userRoadmapId;
        EventType = eventType;
        Metadata = System.Text.Json.JsonSerializer.Serialize(metadata);
    }
}