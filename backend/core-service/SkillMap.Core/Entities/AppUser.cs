
using SkillMap.Core.RoadmapBookmarks;

namespace SkillMap.Core.Entities;

public class AppUser : TrackedEntity
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public virtual ICollection<RoadmapBookmark> UserRoadmaps { get; set; }
}