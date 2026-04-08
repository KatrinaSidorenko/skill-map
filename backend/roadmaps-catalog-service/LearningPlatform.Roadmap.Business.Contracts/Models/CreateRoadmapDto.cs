namespace LearningPlatform.Roadmap.Business.Contracts.Models;
public class CreateRoadmapDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string OwnerId { get; set; }
    public string SourceId { get; set; }
    public int SourceVersion { get; set; }

    public List<NodeDto> Nodes { get; set; }
    public List<EdgeDto> Edges { get; set; }
}