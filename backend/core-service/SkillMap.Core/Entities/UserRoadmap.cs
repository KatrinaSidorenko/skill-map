namespace SkillMap.Core.Entities;

public class UserRoadmap : TrackedEntity
{
    public long UserId { get; set; }
    public string RoadmapId { get; set; }
    public bool IsActive { get; set; }
    public bool IsOwner { get; set; }

    public virtual AppUser User { get; set; }
    public virtual ICollection<RoadmapModification> RoadmapModifications { get; set; }
    public virtual ICollection<RoadmapSnapshot> RoadmapSnapshots { get; set; }
    public virtual ICollection<UserRoadmapTest.UserRoadmapTest> UserRoadmapTests { get; set; }
}
