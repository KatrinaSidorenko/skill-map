using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;

[UsedImplicitly]
internal sealed class CreateLearningItemConnectionCommandHandler(
    IRoadmapWorkspaceEventRepository repository, 
    IRoadmapWorkspaceEditor roadmapWorkspaceEditor,
    IEventBus eventBus) : IRequestHandler<CreateLearningItemConnectionCommand>
{
    public async Task Handle(CreateLearningItemConnectionCommand command, CancellationToken cancellationToken)
    {
        var actualSnapshot = await roadmapWorkspaceEditor.GetActualRoadmapWorkspaceSnapshot(command.WorkspaceId, cancellationToken);
        var @event = command.ToRoadmapWorkspaceEvent();
        var candidateSnapshot = await actualSnapshot.ApplyEventsToSnapshot([@event], cancellationToken);
        if (TopologicalSort.HasCycle(candidateSnapshot))
        {
            throw new InvalidOperationException("Adding this connection would create a cycle in the roadmap.");
        }

        await repository.AddAsync(@event, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventBus.PublishAsync(RoadmapWorkspaceChangedEvent.Create(command.WorkspaceId, @event.Version, command.EventType), cancellationToken);
    }
}