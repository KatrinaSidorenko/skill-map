using Newtonsoft.Json;

namespace SkillMap.Core.RoadmapsWorkspace.Events;

public class LearningItemUpdatedEvent : IWorkspaceEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("title")]
    public string? Title { get; set; }
    
    [JsonProperty("description")]
    public string? Description { get; set; }
    
    [JsonProperty("status")]
    public string? Status { get; set; }

    public LearningItemUpdatedEvent(string id, string? title = null, string? description = null, string? status = null)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
    }
}
