using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Extensions;


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

    public static EdgeDto GenerateInnerId(this EdgeDto edgeDto)
    {
        if (edgeDto == null)
            throw new ArgumentNullException(nameof(edgeDto));

        edgeDto.Id = Guid.NewGuid().ToString();
        return edgeDto;
    }
}
