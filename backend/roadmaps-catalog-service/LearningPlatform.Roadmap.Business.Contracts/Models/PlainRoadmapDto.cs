namespace LearningPlatform.Roadmap.Business.Contracts.Models;

public class PlainRoadmapDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalTopics { get; set; }
}
