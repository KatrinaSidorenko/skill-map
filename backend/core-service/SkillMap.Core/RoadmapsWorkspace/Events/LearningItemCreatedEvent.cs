using Newtonsoft.Json;

namespace SkillMap.Core.RoadmapsWorkspace.Events;

public class LearningItemCreatedEvent : IWorkspaceEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }

    public LearningItemCreatedEvent(string id, string title, string? description,  string status, string type)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        Type = type;
    }
}