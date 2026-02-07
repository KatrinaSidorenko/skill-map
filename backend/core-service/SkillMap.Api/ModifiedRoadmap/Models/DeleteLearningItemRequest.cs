using Newtonsoft.Json;

namespace SkillMap.Api.ModifiedRoadmap.Models;

public class DeleteLearningItemRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
}