
using SkillMap.Shared.Models;

namespace SkillMap.Application.Helpers;

public static class EdgesValidator
{
    public static bool IsValid(this EdgeDto<NodeDto> edge)
        => edge != null && edge.Source != null && edge.Target != null;
}
