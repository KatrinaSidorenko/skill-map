using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapBookmarks.IntegrationEvents;
public record RoadmapBookmarkAddedEvent(Guid Id, long UserRoadmapId, string RoadmapId, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static RoadmapBookmarkAddedEvent Create(long userRoadmapId, string roadmapId)
        => new (Guid.NewGuid(), userRoadmapId, roadmapId, DateTimeOffset.UtcNow);
}