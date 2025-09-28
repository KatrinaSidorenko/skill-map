using LearningPlatform.Roadmap.Business.Contracts.Models;

namespace LearningPlatform.Roadmap.Business.Helpers;

public static class EdgesValidator
{
    public static bool IsValid(this EdgeDto edge)
        => edge != null && edge.Source != null && edge.Target != null;
}
