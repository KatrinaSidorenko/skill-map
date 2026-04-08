using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
public record PersonalRoadmapPublishedEvent(Guid Id, long WorkspaceId, long AuthorId, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static PersonalRoadmapPublishedEvent Create(long workspaceId, long authorId)
        => new(Guid.NewGuid(), workspaceId, authorId, DateTimeOffset.UtcNow);
}