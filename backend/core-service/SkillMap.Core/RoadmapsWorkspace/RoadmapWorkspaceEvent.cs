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
    public WorkspaceEventType EventType { get; private set; }
    public string? Metadata { get; private set; }
    public int Version { get; private set; }

    public virtual RoadmapWorkspace RoadmapWorkspace { get; set; }
    public RoadmapWorkspaceEvent() { }
    public RoadmapWorkspaceEvent(long userRoadmapId, WorkspaceEventType eventType, string metadata, int version)
    {
        if (metadata == null)
        {
            throw new BusinessRuleException(RuleName.PersonalizeRoadmapEventMetadataCannotBeNull);
        }

        RoadmapWorkspaceId = userRoadmapId;
        EventType = eventType;
        Metadata = metadata;
        Version = version;
    }
}