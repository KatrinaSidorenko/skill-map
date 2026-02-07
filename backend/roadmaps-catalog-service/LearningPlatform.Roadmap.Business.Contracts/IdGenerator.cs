using LearningPlatform.Roadmap.Business.Contracts.Models;

using SkillMap.Shared.Extensions;


namespace LearningPlatform.Roadmap.Business.Contracts;

public static class IdGenerator
{
    public static string RemoveDashFromGuid(this string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
        return id.Replace("-", string.Empty);
    }
    public static NodeDto GenerateInnerId(this NodeDto nodeDto)
    {
        if (nodeDto == null)
            throw new ArgumentNullException(nameof(nodeDto));

        if (nodeDto.Id != null)
            return nodeDto;

        var copy = nodeDto.DeepCopy();
        copy.Id = GenerateNewId();
        return copy;
    }

    public static string GenerateNewId() => Guid.NewGuid().ToString("N");
    public static EdgeDto GenerateInnerId(this EdgeDto edgeDto)
    {
        if (edgeDto == null)
            throw new ArgumentNullException(nameof(edgeDto));

        edgeDto.Id = GenerateNewId();
        return edgeDto;
    }
}