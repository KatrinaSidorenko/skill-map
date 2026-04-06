using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
public record PersonalRoadmapCreatedEvent(Guid Id, long UserId, long RoadmapId, string Title, string Description, string ImageUrl, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static PersonalRoadmapCreatedEvent Create(long userId, long roadmapId, string title, string description, string imageUrl)
        => new(Guid.NewGuid(), userId, roadmapId, title, description, imageUrl, DateTimeOffset.UtcNow);
}