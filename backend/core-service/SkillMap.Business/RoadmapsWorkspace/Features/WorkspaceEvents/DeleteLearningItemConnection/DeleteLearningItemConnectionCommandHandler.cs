using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItemConnection;

[UsedImplicitly]
internal sealed class DeleteLearningItemConnectionCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<DeleteLearningItemConnectionCommand>
{
    public async Task Handle(DeleteLearningItemConnectionCommand command, CancellationToken cancellationToken)
    {
        var @event = command.ToRoadmapWorkspaceEvent();
        await repository.AddAsync(@event, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventBus.PublishAsync(RoadmapWorkspaceChangedEvent.Create(command.WorkspaceId, @event.Version, command.EventType), cancellationToken);
    }
}