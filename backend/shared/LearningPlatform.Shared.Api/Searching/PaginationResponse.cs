using Newtonsoft.Json;

namespace LearningPlatform.Shared.Api.Searching;

public class PaginationResponse<T>
{
    [JsonProperty("total")]
    public int Total { get; set; }
    [JsonProperty("items")]
    public List<T> Items { get; set; }
}
