namespace SkillMap.Business.__old.UserRoadmaps.Models;

public class UserRoadmapDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string RoadmapId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}