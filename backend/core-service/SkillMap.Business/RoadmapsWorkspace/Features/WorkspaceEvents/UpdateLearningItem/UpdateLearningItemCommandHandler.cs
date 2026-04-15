using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.UpdateLearningItem;

[UsedImplicitly]
internal sealed class UpdateLearningItemCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<UpdateLearningItemCommand>
{
    public async Task Handle(UpdateLearningItemCommand command, CancellationToken cancellationToken)
    {
        var lastVersion = await repository.GetLastAvailableEventVersion(command.WorkspaceId, cancellationToken, withIncrement: true);
        await repository.AddAsync(command.ToRoadmapWorkspaceEvent(lastVersion), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(command.GetItemStatusProjectionCommand(), cancellationToken);
    }
}