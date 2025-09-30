namespace SkillMap.Shared.Models;

public record PaginationParams(int PageNumber, int PageSize)
{
    public int Skip => (PageNumber - 1) * PageSize;
}
