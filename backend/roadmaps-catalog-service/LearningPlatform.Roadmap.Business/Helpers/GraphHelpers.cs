using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;

namespace LearningPlatform.Roadmap.Business.Helpers;

public static class GraphHelpers
{
    public static bool IsCyclic(this List<List<NodeDto>> sccNodes)
    {
        return sccNodes.Any(component => component.Count > 1);
    }

    public static List<List<NodeDto>> GetSCCs(List<List<NodeDto>> sccNodes)
    {
        return sccNodes
            .Where(component => component.Count > 1)
            .Select(component => component.ToList())
            .ToList();
    }

    public static List<EdgeDto> ResolveCycles(this List<List<NodeDto>> sccComponents, List<EdgeDto> edges)
    {
        var cycleEdges = new List<EdgeDto>();

        foreach (var component in sccComponents)
        {
            var componentEdges = edges.Where(e => component.Contains(e.Source) && component.Contains(e.Target)).ToList();
            
            cycleEdges.AddRange(componentEdges.Where(e => !e.IsCorrectEdgeDirection()));
        }

        return cycleEdges;
    }

    private static bool IsCorrectEdgeDirection(this EdgeDto edge)
        => edge.Source.Type.IsTopic() && edge.Target.Type.IsSubTopic();
        //|| edge.Source.Type.IsSubTopic() && edge.Target.Type.IsSubTopic()
        //|| edge.Source.Type.IsTopic() && edge.Target.Type.IsTopic();

    public static NodeDto GetRootNode(this List<EdgeDto> edges)
    {
        var sources = edges.Select(e => e.Source).Where(s => s != null).ToList();
        var targets = edges.Select(e => e.Target).Where(t => t != null).ToHashSet();

        return sources.FirstOrDefault(s => !targets.Any(t => t.ExternalId == s.ExternalId));
    }
}
