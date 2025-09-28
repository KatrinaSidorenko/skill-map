using SkillMap.Application.OutPorts.Persistence;
using SkillMap.Shared.Extensions;


namespace SkillMap.Persistence.Neo4j.Helpers;

public static class IdGenerator
{
    public static NodeDao GenerateInnerId(this NodeDao nodeDto)
    {
        if (nodeDto == null)
            throw new ArgumentNullException(nameof(nodeDto));

        if (nodeDto.Id != null)
            return nodeDto;

        var copy = nodeDto.DeepCopy();
        copy.Id = Guid.NewGuid().ToString();
        return copy;
    }

    public static EdgeDao<NodeDao> GenerateInnerId(this EdgeDao<NodeDao> edgeDto)
    {
        if (edgeDto == null)
            throw new ArgumentNullException(nameof(edgeDto));

        edgeDto.Id = Guid.NewGuid().ToString();
        return edgeDto;
    }
}
