namespace SkillMap.Business.RoadmapAssessments.Common;

internal sealed class NodeMeta
{
    public LeaningItemAssessment Node { get; }
    public int InDegree { get; set; }
    public int OutDegree { get; set; }
    public int Depth { get; set; }
    public HashSet<string> Ancestors { get; } = new();
    public HashSet<string> Descendants { get; } = new();
    public double StructuralInfluence { get; set; }

    public NodeMeta(LeaningItemAssessment node)
    {
        Node = node;
    }
}

internal sealed class AssessmentGraph
{
    public IReadOnlyList<LeaningItemAssessment> Nodes { get; init; }
    public IReadOnlyList<LearningItemsConnectionAssessment> Edges { get; init; }
}