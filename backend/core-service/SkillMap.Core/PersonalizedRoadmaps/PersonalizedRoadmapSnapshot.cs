using SkillMap.Core.Entities;

namespace SkillMap.Core.PersonalizedRoadmaps;

public class PersonalizedRoadmapSnapshot : TrackedEntity
{
    public long UserRoadmapId { get; set; }
    public byte[] Content { get; set; } // gzipped JSON
    public int LatestVersion { get; set; }

    public virtual UserRoadmap UserRoadmap { get; set; }
}