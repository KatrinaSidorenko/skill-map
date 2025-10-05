using Newtonsoft.Json;

namespace SkillMap.Api.ModifiedRoadmap.Models;

public class LearningItemChangeRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
}
