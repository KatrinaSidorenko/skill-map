using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;

public record RoadmapWorkspaceChangedEvent(Guid Id, long WorkspaceId, int NewVersion, WorkspaceEventType EventType, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static RoadmapWorkspaceChangedEvent Create(long workspaceId, int newVersion, WorkspaceEventType eventType)
        => new RoadmapWorkspaceChangedEvent(Guid.NewGuid(), workspaceId, newVersion, eventType, DateTimeOffset.UtcNow);
}