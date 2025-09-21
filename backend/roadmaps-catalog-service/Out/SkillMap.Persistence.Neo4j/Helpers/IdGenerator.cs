

using SkillMap.Shared.Extensions;
using SkillMap.Shared.Models;

namespace SkillMap.Persistence.Neo4j.Helpers;

public static class IdGenerator
{
    public static NodeDto GenerateInnerId(this NodeDto nodeDto)
    {
        if (nodeDto == null)
            throw new ArgumentNullException(nameof(nodeDto));

        if (nodeDto.Id != null)
            return nodeDto;

        var copy = nodeDto.DeepCopy();
        copy.Id = Guid.NewGuid().ToString();
        return copy;
    }

    public static EdgeDto<NodeDto> GenerateInnerId(this EdgeDto<NodeDto> edgeDto)
    {
        if (edgeDto == null)
            throw new ArgumentNullException(nameof(edgeDto));

        edgeDto.Id = Guid.NewGuid().ToString();
        return edgeDto;
    }
}
