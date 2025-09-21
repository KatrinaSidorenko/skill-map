using SkillMap.Api.Models;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Models;

namespace SkillMap.Api.Mappers;

public static class RoadmapsMapper
{
    public static List<RoadmapResponse> ToRoadmaps(this List<NodeDto> nodes)
    {
        return nodes.Select(node => node.ToRoadmap()).ToList();
    }

    public static RoadmapResponse ToRoadmap(this NodeDto node)
    {
        return new RoadmapResponse
        {
            Id = node.Id,
            Title = node.Title.FirstCharToUpper(),
        };
    }

    public static List<NodeResponse> BuildNodes(this IEnumerable<NodeDto> nodes, Dictionary<NodeDto, List<NodeDto>> adjacencyList, List<string> orderedNodeIds)
    {
        return nodes.Select(node => node.ToNode(adjacencyList, orderedNodeIds))
            .OrderBy(n => orderedNodeIds.IndexOf(n.Id)).ToList();
    }

    public static NodeResponse ToNode(this NodeDto node, Dictionary<NodeDto, List<NodeDto>> adjacencyList, List<string> orderedNodeIds)
    {
        var children = adjacencyList.GetOrDefault(node)?
            .Select(n => n.ToNode(adjacencyList, orderedNodeIds))
            .OrderBy(n => orderedNodeIds.IndexOf(n.Id))
            .ToList() ?? [];

        return new NodeResponse
        {
            Id = node.Id,
            Title = node.Title.FirstCharToUpper(),
            Description = node.Description,
            Type = node.Type,
            Children = children,
            Index = orderedNodeIds.IndexOf(node.Id),
            ParentId = node.Id,
            AdditionalProps = node.AdditionalProps,
        };
    }

    public static PlainNodeResponse ToPlainNode(this NodeDto node, List<string> ids)
    {
        return new PlainNodeResponse
        {
            Id = node.Id,
            Title = node.Title.FirstCharToUpper(),
            Description = node.Description,
            Type = node.Type,
            Index = ids.IndexOf(node.ExternalId),
            ParentId = null,
            AdditionalProps = node.AdditionalProps,
        };
    }

    public static PlainEdgeResponse ToPlainEdge(this EdgeDto<NodeDto> edge)
    {
        return new PlainEdgeResponse
        {
            Source = edge.Source.Id,
            Target = edge.Target.Id,
        };
    }
}
