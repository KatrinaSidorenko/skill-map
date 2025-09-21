namespace SkillMap.Core.Entities;

public class RoadmapSnapshot : TrackedEntity
{
    public long UserRoadmapId { get; set; }
    public byte[] Content { get; set; } // gzipped JSON

    public virtual UserRoadmap UserRoadmap { get; set; }
}
