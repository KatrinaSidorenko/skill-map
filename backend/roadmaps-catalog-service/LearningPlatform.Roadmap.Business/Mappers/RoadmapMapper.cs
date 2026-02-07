using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;

using SkillMap.Shared.Extensions;

namespace LearningPlatform.Roadmap.Business.Mappers;

public static class RoadmapMapper
{
    public static PlainRoadmapDto ToPlainRoadmap(this NodeDto nodeDto)
    {
        if (nodeDto == null)
            throw new ArgumentNullException(nameof(nodeDto));
        return new PlainRoadmapDto
        {
            Id = nodeDto.Id,
            Title = nodeDto.Title,
            Description = nodeDto.Description,
            ImageUrl = nodeDto.AdditionalProps?.GetOrDefault(NodeProps.ImageUrl),
            OwnerId = nodeDto.AdditionalProps?.GetOrDefault(NodeProps.OwnerId),
            IsPublic = nodeDto.AdditionalProps?.GetOrDefault(NodeProps.IsPublic) == NodeProps.True,
        };
    }

    public static NodeDto ToNodeDto(this PlainRoadmapDto plainRoadmapDto)
    {
        if (plainRoadmapDto == null)
            throw new ArgumentNullException(nameof(plainRoadmapDto));
        return new NodeDto
        {
            Id = plainRoadmapDto.Id ?? IdGenerator.GenerateNewId(),
            Title = plainRoadmapDto.Title,
            Description = plainRoadmapDto.Description,
            Type = NodeType.Roadmap,
            AdditionalProps = new Dictionary<string, string>
            {
                { NodeProps.ImageUrl, plainRoadmapDto.ImageUrl ?? string.Empty },
                { NodeProps.OwnerId, plainRoadmapDto.OwnerId ?? string.Empty },
                { NodeProps.IsPublic, plainRoadmapDto.IsPublic ? NodeProps.True : NodeProps.False },
            }
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