using Newtonsoft.Json;

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
            WorkspaceEventType.CreateLearningItem => JsonConvert.DeserializeObject<LearningItemCreatedEvent>(rawEvent.Metadata),

            WorkspaceEventType.UpdateTitle or
            WorkspaceEventType.UpdateDescription or
            WorkspaceEventType.UpdatePriority or
            WorkspaceEventType.UpdateStatus or
            WorkspaceEventType.UpdateLearningItem => JsonConvert.DeserializeObject<LearningItemUpdatedEvent>(rawEvent.Metadata),
            WorkspaceEventType.DeleteLearningItem => JsonConvert.DeserializeObject<LearningItemDeletedEvent>(rawEvent.Metadata),

            WorkspaceEventType.CreateConnection => JsonConvert.DeserializeObject<LearningItemConnectionCreatedEvent>(rawEvent.Metadata),
            WorkspaceEventType.DeleteConnection => JsonConvert.DeserializeObject<LearningItemConnectionDeletedEvent>(rawEvent.Metadata),

            _ => null
        };
    }
}