using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItem;

[UsedImplicitly]
internal sealed class DeleteLearningItemCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<DeleteLearningItemCommand>
{
    public async Task Handle(DeleteLearningItemCommand command, CancellationToken cancellationToken)
    {
        var lastVersion = await repository.GetLastAvailableEventVersion(command.WorkspaceId, cancellationToken, withIncrement: true);
        var deleteEvent = new RoadmapWorkspaceEvent(command.WorkspaceId, command.EventType, command.GetMetadataJson(), lastVersion);
        await repository.AddAsync(deleteEvent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapWorkspaceChangedEvent.Create(command.WorkspaceId, lastVersion, command.EventType), cancellationToken);
    }
}
