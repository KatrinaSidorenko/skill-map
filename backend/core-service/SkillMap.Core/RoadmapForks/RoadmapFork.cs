using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.User;

namespace SkillMap.Core.RoadmapBookmarks;

public class RoadmapFork : TrackedEntity
{
    public long AuthorId { get; private set; }
    public string RoadmapId { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsInAuthorMode { get; private set; }

    public virtual AppUser User { get; set; }
    public virtual ICollection<RoadmapWorkspaceEvent> WorkspaceEvents { get; set; }
    public virtual ICollection<RoadmapWorkspaceSnapshot> Snapshots { get; set; }
    public virtual ICollection<RoadmapAssessment> Assessments { get; set; }

    public RoadmapFork() { }
    public RoadmapFork(long userId, string roadmapId)
    {
        AuthorId = userId;
        RoadmapId = roadmapId;
        IsActive = true;
        IsInAuthorMode = false;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ActivateAuthorMode()
    {
        IsInAuthorMode = true;
    }
}