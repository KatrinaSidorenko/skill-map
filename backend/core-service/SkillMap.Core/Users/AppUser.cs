using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Core.User;

public class AppUser : TrackedEntity
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public string? ImageUrl { get; set; }
    public virtual ICollection<RoadmapWorkspace> RoadmapForks { get; set; }
}