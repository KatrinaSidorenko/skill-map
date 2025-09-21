namespace SkillMap.Business.UserRoadmaps.Models;

public class UserRoadmapDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string RoadmapId { get; set; }
    public bool IsActive { get; set; }
}
