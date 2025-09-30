namespace SkillMap.Shared.Models;

public class PaginationResult<T>
{
    public T Result { get; set; }
    public int TotalCount { get; set; }
}
