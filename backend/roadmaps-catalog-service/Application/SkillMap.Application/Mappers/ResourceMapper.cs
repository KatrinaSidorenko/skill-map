using SkillMap.Shared;
using SkillMap.Shared.Models;

namespace SkillMap.Application.Mappers;

public static class ResourceMapper
{
    public static NodeDto ToNode(this ResourceDto resourceDto)
    {
        return new NodeDto
        {
            //Id = resourceDto.Id,
            Id = Guid.NewGuid().ToString(),
            Title = resourceDto.Title,
            Type = NodeType.Resource,
            AdditionalProps = new Dictionary<string, string>
            {
                { "resource_link", resourceDto.Link },
                { "resource_type", resourceDto.Type }
            }
        };
    }
}
