namespace LearningPlatform.Roadmap.Business.Contracts.Models;

public class ResourceDto : IEquatable<ResourceDto>
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public string Type { get; set; }

    public bool Equals(object? obj)
    {
        if (obj is ResourceDto other)
        {
            return Equals(other);
        }
        return false;
    }

    public bool Equals(ResourceDto? other)
    {
        if (other == null)
            return false;

        return Id == other.Id &&
               Title == other.Title &&
               Link == other.Link &&
               Type == other.Type;

    }

    public override int GetHashCode() => HashCode.Combine(Id, Title, Link, Type);
}
