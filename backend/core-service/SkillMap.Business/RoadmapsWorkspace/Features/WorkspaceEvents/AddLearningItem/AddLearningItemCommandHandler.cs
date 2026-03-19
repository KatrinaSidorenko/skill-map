using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.AddLearningItem;

[UsedImplicitly]
internal sealed class AddLearningItemCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<AddLearningItemCommand>
{
    public async Task Handle(AddLearningItemCommand command, CancellationToken cancellationToken)
    {
        var lastVersion = await repository.GetLastAvailableEventVersion(command.WorkspaceId, cancellationToken, withIncrement: true);
        var addEvent = new RoadmapWorkspaceEvent(command.WorkspaceId, command.EventType, command.GetMetadataJson(), lastVersion);
        await repository.AddAsync(addEvent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapWorkspaceChangedEvent.Create(command.WorkspaceId, lastVersion, command.EventType), cancellationToken);
    }
}
