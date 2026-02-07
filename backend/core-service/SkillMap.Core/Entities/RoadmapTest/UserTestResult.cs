namespace SkillMap.Core.Entities.UserRoadmapTest;

public class UserTestResult : TrackedEntity
{
    public long UserRoadmapTestId { get; set; }
    public double MaxPoints { get; set; }
    public double ScoredPoints { get; set; }
    public byte[] ResultData { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public virtual UserRoadmapTest UserRoadmapTest { get; set; }
}