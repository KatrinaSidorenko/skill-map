using JetBrains.Annotations;

using MediatR;

using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.UpdateLearningItem;

[UsedImplicitly]
internal sealed class UpdateLearningItemCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<UpdateLearningItemCommand>
{
    public async Task Handle(UpdateLearningItemCommand command, CancellationToken cancellationToken)
    {
        await repository.AddAsync(command.ToRoadmapWorkspaceEvent(), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(command.GetItemStatusProjectionCommand(), cancellationToken);
    }
}