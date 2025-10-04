using SkillMap.Core.Constants;

namespace SkillMap.Business.ModifiedRoadmaps.Models;

public class PlainRoadmapWithDetailsDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string? ImageUrl { get; set; }
    public double Progress { get; set; }
    public DateTime SavedAt { get; set; }
    public LearningStatus Status { get; set; }
}
