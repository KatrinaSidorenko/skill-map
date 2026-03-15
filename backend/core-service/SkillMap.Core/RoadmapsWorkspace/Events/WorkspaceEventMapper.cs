using Newtonsoft.Json;

using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Core.RoadmapsWorkspace.Events;
public static class WorkspaceEventMapper
{
    public static IWorkspaceEvent? Map(RoadmapWorkspaceEvent rawEvent)
    {
        if (string.IsNullOrWhiteSpace(rawEvent.Metadata))
            return null;

        return rawEvent.EventType switch
        {
            EventType.CreateLearningItem => JsonConvert.DeserializeObject<LearningItemCreatedEvent>(rawEvent.Metadata),

            EventType.UpdateTitle or
            EventType.UpdateDescription or
            EventType.UpdatePriority or
            EventType.UpdateStatus or
            EventType.UpdateLearningItem => JsonConvert.DeserializeObject<LearningItemUpdatedEvent>(rawEvent.Metadata),
            EventType.DeleteItem => JsonConvert.DeserializeObject<LearningItemDeletedEvent>(rawEvent.Metadata),

            EventType.CreateConnection => JsonConvert.DeserializeObject<LearningItemConnectionCreatedEvent>(rawEvent.Metadata),

            EventType.DeleteConnection => JsonConvert.DeserializeObject<LearningItemConnectionDeletedEvent>(rawEvent.Metadata),

            _ => null
        };
    }
}