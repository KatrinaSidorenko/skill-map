using Newtonsoft.Json;

using SkillMap.Shared.Models;

namespace LearningPlatform.Shared.Api.Searching;

public class FilteringRequest : PaginationRequest
{
    [JsonProperty("query")]
    public string? Query { get; set; }

    public new FilteringParams ToParams() => new FilteringParams(Query, new PaginationParams(PageNumber ?? 1, PageSize ?? 10));
}