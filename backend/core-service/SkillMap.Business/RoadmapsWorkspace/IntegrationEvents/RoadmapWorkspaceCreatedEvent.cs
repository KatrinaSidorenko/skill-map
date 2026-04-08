using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
public record RoadmapWorkspaceCreatedEvent(Guid Id, long WorkspaceId, string RoadmapId, bool IsInAuthorMode, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static RoadmapWorkspaceCreatedEvent Create(long workspaceId, string roadmapId, bool isInAuthorMode)
        => new(Guid.NewGuid(), workspaceId, roadmapId, isInAuthorMode, DateTimeOffset.UtcNow);
}