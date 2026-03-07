using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalizedRoadmaps.IntegrationEvents;
public record ForkRoadmapEvent(Guid Id, long UserRoadmapId, string RoadmapId, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static ForkRoadmapEvent Create(long userRoadmapId, string roadmapId)
        => new (Guid.NewGuid(), userRoadmapId, roadmapId, DateTime.UtcNow);
}