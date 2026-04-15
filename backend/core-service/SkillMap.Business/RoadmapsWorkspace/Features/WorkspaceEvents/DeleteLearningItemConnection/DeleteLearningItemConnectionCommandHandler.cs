using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItemConnection;

[UsedImplicitly]
internal sealed class DeleteLearningItemConnectionCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<DeleteLearningItemConnectionCommand>
{
    public async Task Handle(DeleteLearningItemConnectionCommand command, CancellationToken cancellationToken)
    {
        await repository.AddAsync(command.ToRoadmapWorkspaceEvent(), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}