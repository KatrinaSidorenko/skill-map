using Newtonsoft.Json;

namespace SkillMap.Core.RoadmapsWorkspace.Events;
public class LearningItemDeletedEvent : IWorkspaceEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }

    public LearningItemDeletedEvent(string id)
    {
        Id = id;
    }
}