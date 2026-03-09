using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.Roadmaps;
using SkillMap.Core.User;

namespace SkillMap.Core.RoadmapsWorkspace;

public class RoadmapWorkspace : TrackedEntity
{
    public long AuthorId { get; private set; }
    public string? RoadmapId { get; private set; }
    public long? PersonalRoadmapId { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsInAuthorMode { get; private set; }

    public virtual AppUser Author { get; set; }
    public virtual PersonalRoadmap? PersonalRoadmap { get; set; }
    public virtual ICollection<RoadmapWorkspaceEvent> WorkspaceEvents { get; set; }
    public virtual ICollection<RoadmapWorkspaceSnapshot> Snapshots { get; set; }
    public virtual ICollection<RoadmapAssessment> Assessments { get; set; }


    public RoadmapWorkspace() { }
    public RoadmapWorkspace(long userId, string? roadmapId, long? personalRoadmapId)
    {
        AuthorId = userId;
        RoadmapId = roadmapId;
        PersonalRoadmapId = personalRoadmapId;
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