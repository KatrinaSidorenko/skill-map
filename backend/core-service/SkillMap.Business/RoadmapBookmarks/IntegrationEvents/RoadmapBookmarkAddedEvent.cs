using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapBookmarks.IntegrationEvents;
public record RoadmapBookmarkAddedEvent(Guid Id, long BookmarkId, string RoadmapId, bool IsSourceRoadmapAvailable, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static RoadmapBookmarkAddedEvent Create(long userRoadmapId, string roadmapId, bool isInAuthorMode)
        => new (Guid.NewGuid(), userRoadmapId, roadmapId, isInAuthorMode, DateTimeOffset.UtcNow);
}