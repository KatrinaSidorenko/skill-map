using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.AddLearningItem;

[UsedImplicitly]
internal sealed class AddLearningItemCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<AddLearningItemCommand>
{
    public async Task Handle(AddLearningItemCommand command, CancellationToken cancellationToken)
    {
        var @event = command.ToRoadmapWorkspaceEvent();
        await repository.AddAsync(@event, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(command.GetItemStatusProjectionCommand(), cancellationToken);
        await eventBus.PublishAsync(RoadmapWorkspaceChangedEvent.Create(command.WorkspaceId, @event.Version, command.EventType), cancellationToken);
    }
}