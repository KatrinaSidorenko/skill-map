using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Core.RoadmapAssessments;

public class RoadmapAssessment : TrackedEntity
{
    public long RoadmapWorkspaceId { get; set; }
    public string TestType { get; set; }
    public byte[] TestData { get; set; }
    public virtual RoadmapWorkspace RoadmapFork { get; set; }
}