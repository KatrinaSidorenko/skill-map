using Newtonsoft.Json;

namespace SkillMap.Core.RoadmapsWorkspace.Events;
public class LearningItemDeletedEvent : IWorkspaceEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("incidentConnectionIds")]
    public List<string> IncidentConnectionIds { get; set; }

    public LearningItemDeletedEvent(string id, List<string> incidentConnectionIds)
    {
        Id = id;
        IncidentConnectionIds = incidentConnectionIds;
    }
}