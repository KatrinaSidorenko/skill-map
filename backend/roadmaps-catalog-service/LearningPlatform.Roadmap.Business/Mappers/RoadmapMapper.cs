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
            ImageUrl = nodeDto.AdditionalProps?.GetOrDefault(NodeType.ImageUrl)
        };
    }

    public static List<PlainRoadmapDto> ToPlainRoadmaps(this IEnumerable<NodeDto> nodes)
    {
        if (nodes == null)
            throw new ArgumentNullException(nameof(nodes));
        return nodes.Select(ToPlainRoadmap).ToList();
    }
}
