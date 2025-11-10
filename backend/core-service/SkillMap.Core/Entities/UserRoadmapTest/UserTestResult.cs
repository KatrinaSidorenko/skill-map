namespace SkillMap.Core.Entities.UserRoadmapTest;

public class UserTestResult : TrackedEntity
{
    public long UserRoadmapTestId { get; set; }
    public int MaxPoints { get; set; }
    public int ScoredPoints { get; set; }
    public RoadmapTestResult ResultData { get; set; }
    public DateTime CompletedAt { get; set; }
    public virtual UserRoadmapTest UserRoadmapTest { get; set; }
}
