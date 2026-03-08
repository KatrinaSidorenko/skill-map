using JetBrains.Annotations;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
using SkillMap.Business.RoadmapBookmarks.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapBookmarks.Features.PersonalRoadmapCreated;

[UsedImplicitly]
internal sealed class PersonalRoadmapCreatedEventHandler(IRepository<RoadmapWorkspace> repository, IEventBus eventBus) : IIntegrationEventHandler<PersonalRoadmapCreatedEvent>
{
    public async Task Handle(PersonalRoadmapCreatedEvent notification, CancellationToken cancellationToken)
    {
        var roadmapBookmark = new RoadmapWorkspace(notification.UserId, notification.RoadmapId);
        roadmapBookmark.ActivateAuthorMode();

        await repository.AddAsync(roadmapBookmark, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapBookmarkAddedEvent.Create(roadmapBookmark.Id, roadmapBookmark.RoadmapId, false), cancellationToken);
    }
}
