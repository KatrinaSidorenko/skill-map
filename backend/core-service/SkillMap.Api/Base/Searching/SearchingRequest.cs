using Newtonsoft.Json;
using SkillMap.Shared.Models;

namespace SkillMap.Api.Base.Searching;

public class SearchingRequest : PaginationRequest
{
    [JsonProperty("query")]
    public string? Query { get; set; }

    public new SearchingParams ToParams() => new SearchingParams(Query, new PaginationParams(PageNumber, PageSize));
}
