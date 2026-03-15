using Newtonsoft.Json;

namespace SkillMap.Core.RoadmapsWorkspace.Events;
public class LearningItemConnectionDeletedEvent : IWorkspaceEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }

    public LearningItemConnectionDeletedEvent(string id)
    {
        Id = id;
    }
}
