using JetBrains.Annotations;

using MediatR;

using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;

[UsedImplicitly]
internal sealed class CreateLearningItemConnectionCommandHandler(IRoadmapWorkspaceEventRepository repository, IEventBus eventBus) : IRequestHandler<CreateLearningItemConnectionCommand>
{
    public async Task Handle(CreateLearningItemConnectionCommand command, CancellationToken cancellationToken)
    {
        await repository.AddAsync(command.ToRoadmapWorkspaceEvent(), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}