using Newtonsoft.Json;

namespace SkillMap.Api.Base;

public class PaginationRequest
{
    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; } = 1;
    [JsonProperty("pageSize")]
    public int PageSize { get; set; } = 10;
}
