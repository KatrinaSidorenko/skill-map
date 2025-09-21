using Newtonsoft.Json;

namespace SkillMap.Business.Roadmaps.Models;

public class CreateLearningItemMetadata
{
    [JsonProperty("title")]
    public string Title { get; set; }
   
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("parentId")]
    public string ParentId { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
}
