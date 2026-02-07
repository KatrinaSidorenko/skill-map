using SkillMap.Shared.Extensions;

namespace LearningPlatform.Roadmap.Business.Contracts.Models;

public static class ResourceExtensions
{
    public static ResourceDto ToResourceDto(this Dictionary<string, object> props)
    {
        return new ResourceDto
        {
            Id = props.GetOrDefault("id") as string,
            Title = props.GetOrDefault("title") as string,
            Link = props.GetOrDefault("resource_link") as string,
            Type = props.GetOrDefault("resource_type") as string,
        };
    }
}
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