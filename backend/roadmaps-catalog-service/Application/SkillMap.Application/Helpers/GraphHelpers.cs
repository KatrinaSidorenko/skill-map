using SkillMap.Application.Domain;
using SkillMap.Shared;
using SkillMap.Shared.Models;
using System.Xml.Linq;

namespace SkillMap.Application.Helpers;

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

    public static List<EdgeDto<NodeDto>> ResolveCycles(this List<List<NodeDto>> sccComponents, List<EdgeDto<NodeDto>> edges)
    {
        var cycleEdges = new List<EdgeDto<NodeDto>>();

        foreach (var component in sccComponents)
        {
            var componentEdges = edges.Where(e => component.Contains(e.Source) && component.Contains(e.Target)).ToList();
            
            cycleEdges.AddRange(componentEdges.Where(e => !e.IsCorrectEdgeDirection()));
        }

        return cycleEdges;
    }

    private static bool IsCorrectEdgeDirection(this EdgeDto<NodeDto> edge)
        => edge.Source.Type.IsTopic() && edge.Target.Type.IsSubTopic();
        //|| edge.Source.Type.IsSubTopic() && edge.Target.Type.IsSubTopic()
        //|| edge.Source.Type.IsTopic() && edge.Target.Type.IsTopic();

    public static NodeDto GetRootNode(this List<EdgeDto<NodeDto>> edges)
    {
        var sources = edges.Select(e => e.Source).Where(s => s != null).ToList();
        var targets = edges.Select(e => e.Target).Where(t => t != null).ToHashSet();

        return sources.FirstOrDefault(s => !targets.Any(t => t.ExternalId == s.ExternalId));
    }
}
