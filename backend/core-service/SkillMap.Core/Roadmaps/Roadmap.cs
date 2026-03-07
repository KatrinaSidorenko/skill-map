namespace SkillMap.Core.Roadmaps;
public class Roadmap : TrackedEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public bool IsPublic { get; set; }

    public int Version { get; set; }
}
