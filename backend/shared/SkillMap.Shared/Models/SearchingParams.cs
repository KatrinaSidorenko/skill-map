namespace SkillMap.Shared.Models;

public record SearchingParams(string SearchTermByName, PaginationParams PaginationParams);

public static class DefaultParams
{
    public static readonly SearchingParams SearchingParams = new(string.Empty, new PaginationParams(1, 10));
}
