using Newtonsoft.Json;

namespace SkillMap.Business.Roadmaps.Models;
public class LearningItemChange
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

public class DeleteLearningItemChange
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
}
