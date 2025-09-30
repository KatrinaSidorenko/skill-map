using Newtonsoft.Json;

namespace SkillMap.Api.Base.Pagination;

public abstract class PaginationResponse
{
    [JsonProperty("total")]
    public int TotalCount { get; set; }
}
