using Newtonsoft.Json;

namespace SkillMap.Core.RoadmapsWorkspace.Events;

public class LearningItemConnectionCreatedEvent : IWorkspaceEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("target")]
    public string Target { get; set; }

    public LearningItemConnectionCreatedEvent(string id, string source, string target)
    {
        Id = id;
        Source = source;
        Target = target;
    }
}
