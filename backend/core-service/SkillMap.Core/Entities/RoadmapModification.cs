using SkillMap.Core.Constants;

namespace SkillMap.Core.Entities;

public class RoadmapModification : TrackedEntity
{
    public long UserRoadmapId { get; set; }
    public string? InnerItemId { get; set; }
    public string ExternalItemId { get; set; }
    public ModificationAction Action { get; set; }
    public string? Metadata { get; set; }

    public virtual UserRoadmap UserRoadmap { get; set; }
}