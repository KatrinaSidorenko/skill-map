using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;

[UsedImplicitly]
internal sealed class CreateLearningItemConnectionCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<CreateLearningItemConnectionCommand>
{
    public async Task Handle(CreateLearningItemConnectionCommand command, CancellationToken cancellationToken)
    {
        var lastVersion = await repository.GetLastAvailableEventVersion(command.WorkspaceId, cancellationToken, withIncrement: true);
        var connectionEvent = new RoadmapWorkspaceEvent(command.WorkspaceId, command.EventType, command.GetMetadataJson(), lastVersion);
        await repository.AddAsync(connectionEvent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapWorkspaceChangedEvent.Create(command.WorkspaceId, lastVersion, command.EventType), cancellationToken);
    }
}
