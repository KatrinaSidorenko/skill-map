using JetBrains.Annotations;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateRoadmapWorkspace;

[UsedImplicitly]
internal sealed class PersonalRoadmapCreatedEventHandler(IRepository<RoadmapWorkspace> repository, IEventBus eventBus) : IIntegrationEventHandler<PersonalRoadmapCreatedEvent>
{
    public async Task Handle(PersonalRoadmapCreatedEvent notification, CancellationToken cancellationToken)
    {
        var roadmapBookmark = new RoadmapWorkspace(notification.UserId, null, notification.RoadmapId);
        roadmapBookmark.ActivateAuthorMode();
        roadmapBookmark.SetMetadata(new RoadmapWorkspaceMetadata(notification.Title, notification.Description, notification.ImageUrl));

        await repository.AddAsync(roadmapBookmark, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapWorkspaceCreatedEvent.Create(roadmapBookmark.Id, roadmapBookmark.PersonalRoadmapId.ToString(), isInAuthorMode: true), cancellationToken);
    }
}
