using Newtonsoft.Json;

namespace SkillMap.Api.Base.Searching;

public abstract class PaginationResponse
{
    [JsonProperty("total")]
    public int TotalCount { get; set; }
}
