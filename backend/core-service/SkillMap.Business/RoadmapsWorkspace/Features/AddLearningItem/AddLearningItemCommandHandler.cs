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
        // todo: we need to check versions before adding events
        // if client state is less than current last version , we should reject the command and ask client to sync first
        // else save event and increase version by 1
        var addEvent = new RoadmapWorkspaceEvent(command.UserRoadmapId, command.EventType, command.GetMetadataJson());
        await repository.AddAsync(addEvent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
