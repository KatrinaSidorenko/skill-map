using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Extensions;

namespace LearningPlatform.Roadmap.Business.Mappers;

public static class RoadmapMapper
{
    public static string RemoveDashFromGuid(this string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
        return id.Replace("-", string.Empty);
    }
    public static PlainRoadmapDto ToPlainRoadmap(this NodeDto nodeDto)
    {
        if (nodeDto == null)
            throw new ArgumentNullException(nameof(nodeDto));
        return new PlainRoadmapDto
        {
            Id = nodeDto.Id,
            Title = nodeDto.Title,
            ImageUrl = nodeDto.AdditionalProps?.GetOrDefault(NodeType.ImageUrl)
        };
    }

    public static List<PlainRoadmapDto> ToPlainRoadmaps(this IEnumerable<NodeDto> nodes)
    {
        if (nodes == null)
            throw new ArgumentNullException(nameof(nodes));
        return nodes.Select(ToPlainRoadmap).ToList();
    }

    public static Node ToNode(this NodeDto nodeDto)
    {
        if (nodeDto == null)
            throw new ArgumentNullException(nameof(nodeDto));
        return new Node
        {
            Id = nodeDto.Id.RemoveDashFromGuid(),
            Title = nodeDto.Title,
            Description = nodeDto.Description
        };
    }

    public static List<Node> ToNodes(this IEnumerable<NodeDto> nodeDtos)
    {
        if (nodeDtos == null)
            throw new ArgumentNullException(nameof(nodeDtos));
        return nodeDtos.Select(ToNode).ToList();
    }

    public static Edge ToEdge(this EdgeDto edgeDto)
    {
        if (edgeDto == null)
            throw new ArgumentNullException(nameof(edgeDto));

        return new Edge
        {
            Id = edgeDto.Id,
            Source = edgeDto.Source?.Id.RemoveDashFromGuid(),
            Target = edgeDto.Target?.Id.RemoveDashFromGuid(),
        };
    }

    public static List<Edge> ToEdges(this IEnumerable<EdgeDto> edgeDtos)
    {
        if (edgeDtos == null)
            throw new ArgumentNullException(nameof(edgeDtos));
        return edgeDtos.Select(ToEdge).ToList();
    }
}
