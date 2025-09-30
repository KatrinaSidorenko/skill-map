namespace LearningPlatform.Roadmap.Business.Contracts.Models;

public class EdgeDto
{
    public string Id { get; set; }
    public NodeDto Source { get; set; }
    public NodeDto Target { get; set; }
    public bool Equals(EdgeDto? other)
    {
        if (other == null)
            return false;
        return Id == other.Id &&
               Equals(Source, other.Source) &&
               Equals(Target, other.Target);
    }

    public override bool Equals(object? obj)
    {
        if (obj is EdgeDto other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
         => HashCode.Combine(Id, Source, Target);
}
