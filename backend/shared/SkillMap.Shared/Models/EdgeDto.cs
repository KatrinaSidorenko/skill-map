namespace SkillMap.Shared.Models;

public class EdgeDto<TNode> : IEquatable<EdgeDto<TNode>> where TNode : class
{
    public string Id { get; set; }
    public TNode Source { get; set; }
    public TNode Target { get; set; }

    public bool Equals(EdgeDto<TNode>? other)
    {
        if (other == null)
            return false;
        return Id == other.Id &&
               Equals(Source, other.Source) &&
               Equals(Target, other.Target);
    }

    public override bool Equals(object? obj)
    {
        if (obj is EdgeDto<TNode> other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
         => HashCode.Combine(Id, Source, Target);
}
