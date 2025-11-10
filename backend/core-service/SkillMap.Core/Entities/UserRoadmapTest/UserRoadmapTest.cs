namespace SkillMap.Core.Entities.UserRoadmapTest;

public class UserRoadmapTest : TrackedEntity
{
    public long UserRoadmapId { get; set; }
    public string TestType { get; set; }
    public RoadmapTest TestData { get; set; }
    public virtual UserRoadmap UserRoadmap { get; set; }
}
