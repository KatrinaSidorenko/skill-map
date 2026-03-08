using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
public record RoadmapWorkspaceCreatedEvent(Guid Id, long BookmarkId, string RoadmapId, bool IsSourceRoadmapAvailable, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static RoadmapWorkspaceCreatedEvent Create(long userRoadmapId, string roadmapId, bool isInAuthorMode)
        => new(Guid.NewGuid(), userRoadmapId, roadmapId, isInAuthorMode, DateTimeOffset.UtcNow);
}
