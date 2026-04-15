using JetBrains.Annotations;


using MediatR;

using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;

[UsedImplicitly]
internal sealed class CreateLearningItemConnectionCommandHandler(
    IRoadmapWorkspaceEventRepository repository, 
    IRoadmapWorkspaceEditor roadmapWorkspaceEditor) : IRequestHandler<CreateLearningItemConnectionCommand>
{
    public async Task Handle(CreateLearningItemConnectionCommand command, CancellationToken cancellationToken)
    {
        var actualSnapshot = await roadmapWorkspaceEditor.GetActualRoadmapSnapshot(command.WorkspaceId, cancellationToken);
        var @event = command.ToRoadmapWorkspaceEvent();
        var candidateSnapshot = await actualSnapshot.ApplyEventsToSnapshot([@event], cancellationToken);
        if (TopologicalSort.HasCycle(candidateSnapshot))
        {
            throw new InvalidOperationException("Adding this connection would create a cycle in the roadmap.");
        }

        await repository.AddAsync(@event, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}