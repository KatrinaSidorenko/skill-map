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
        // todo: delete not only node but also the edge connected to the node
        await repository.AddAsync(command.ToRoadmapWorkspaceEvent(), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(command.GetItemStatusProjectionCommand(), cancellationToken);
    }
}