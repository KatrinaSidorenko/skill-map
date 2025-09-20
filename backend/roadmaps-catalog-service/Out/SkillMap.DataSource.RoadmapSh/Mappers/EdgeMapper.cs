using SkillMap.DataSource.RoadmapSh.Api;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Models;

namespace SkillMap.DataSource.RoadmapSh.Mappers;

public static class EdgeMapper
{
    public static bool IsValidEdge(this EdgeDto<NodeDto> edge) => edge != null && edge.Source != null && edge.Target != null;
    public static EdgeDto<NodeDto> ToEdgeDto(this RoadmapEdge edge, Dictionary<string, NodeDto> nodes)
    {
        if (edge == null || edge.Source == null || edge.Target == null)
            return null;

        var sourceNode = nodes.GetOrDefault(edge.Source);
        var targetNode = nodes.GetOrDefault(edge.Target);

        if (sourceNode == null || targetNode == null) { return null; }
            

        return new EdgeDto<NodeDto>
        {
            Source = sourceNode,
            Target = targetNode
        };
    }
}
