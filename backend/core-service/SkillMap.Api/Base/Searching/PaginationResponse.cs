using Newtonsoft.Json;

namespace SkillMap.Api.Base.Searching;

public class PaginationResponse<T>
{
    [JsonProperty("total")]
    public int TotalCount { get; set; }
    [JsonProperty("items")]
    public List<T> Items { get; set; }
}
