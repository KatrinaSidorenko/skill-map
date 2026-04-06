namespace SkillMap.Core.RoadmapAssessments;

public class AssessmentAttempt : TrackedEntity
{
    public long AssessmentId { get; set; }
    public long UserId { get; set; }
    public double MaxPoints { get; set; }
    public double ScoredPoints { get; set; }
    public byte[] ResultData { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public virtual RoadmapAssessment RoadmapAssessment { get; set; }
}