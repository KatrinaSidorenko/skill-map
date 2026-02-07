using Newtonsoft.Json;

using SkillMap.Shared.Models;

namespace LearningPlatform.Shared.Api.Searching;

public class PaginationRequest
{
    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; } = 1;
    [JsonProperty("pageSize")]
    public int PageSize { get; set; } = 10;

    public PaginationParams ToParams() => new PaginationParams(PageNumber, PageSize);
}