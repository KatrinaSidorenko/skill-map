namespace SkillMap.Shared.Models;

public class NodeDto : IEquatable<NodeDto>
{
    private string? _externalId;
    public string Id { get; set; }
    public string ExternalId
    {
        get => _externalId ?? Id;
        set => _externalId = value;
    }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string> AdditionalProps { get; set; }

    public bool Equals(NodeDto? other)
    {
        if (other == null)
            return false;

        return Id == other.Id &&
               ExternalId == other.ExternalId &&
               Title == other.Title &&
               Description == other.Description &&
               Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        if (obj is NodeDto other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(Id, ExternalId, Title, Description, Type);
}
