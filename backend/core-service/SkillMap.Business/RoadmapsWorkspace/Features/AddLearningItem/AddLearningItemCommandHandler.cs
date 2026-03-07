using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.AddLearningItem;

[UsedImplicitly]
internal sealed class AddLearningItemCommandHandler(IRepository<RoadmapWorkspaceEvent> repository) : IRequestHandler<AddLearningItemCommand>
{
    public async Task Handle(AddLearningItemCommand command, CancellationToken cancellationToken)
    {
        var addEvent = new RoadmapWorkspaceEvent(command.UserRoadmapId, command.EventType, command.GetMetadata());
        await repository.AddAsync(addEvent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
