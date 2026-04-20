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
            WorkspaceEventType.LearningItemCreated => JsonConvert.DeserializeObject<LearningItemCreatedEvent>(rawEvent.Metadata),

            WorkspaceEventType.UpdateTitle or
            WorkspaceEventType.UpdateDescription or
            WorkspaceEventType.UpdatePriority or
            WorkspaceEventType.UpdateStatus or
            WorkspaceEventType.LearningItemUpdated => JsonConvert.DeserializeObject<LearningItemUpdatedEvent>(rawEvent.Metadata),
            WorkspaceEventType.LearningItemDeleted => JsonConvert.DeserializeObject<LearningItemDeletedEvent>(rawEvent.Metadata),

            WorkspaceEventType.ConnectionCreated => JsonConvert.DeserializeObject<LearningItemConnectionCreatedEvent>(rawEvent.Metadata),
            WorkspaceEventType.ConnectionDeleted => JsonConvert.DeserializeObject<LearningItemConnectionDeletedEvent>(rawEvent.Metadata),

            _ => null
        };
    }
}