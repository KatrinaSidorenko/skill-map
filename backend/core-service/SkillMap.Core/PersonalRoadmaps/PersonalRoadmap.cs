using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.User;

namespace SkillMap.Core.Roadmaps;
public class PersonalRoadmap : TrackedEntity
{
    public long Id { get; set; }
    public long AuthorId { get; set; }

    public string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPublic { get; set; }

    public virtual AppUser Author { get; set; }
    public virtual RoadmapWorkspace RoadmapWorkspace { get; set; }

    public PersonalRoadmap() { }
    public PersonalRoadmap(long authorId, string title, string description, string imageUrl)
    {
        AuthorId = authorId;
        Title = title;
        Description = description;
        ImageUrl = imageUrl;
        IsPublic = false;
    }

    public void Publish()
    {
        IsPublic = true;
    }
}
