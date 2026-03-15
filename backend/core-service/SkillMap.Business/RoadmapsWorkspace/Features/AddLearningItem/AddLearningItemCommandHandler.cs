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
        var lastEvent = await repository.GetAllAsync(
            filter: e => e.RoadmapWorkspaceId == command.UserRoadmapId,
            orderBy: q => q.OrderByDescending(e => e.CreatedAt).OrderByDescending(e => e.Version),
            count: 1,
            ct: cancellationToken); // todo: it can be optimized

        var lastVersion = lastEvent.FirstOrDefault()?.Version + 1 ?? 0;
        var addEvent = new RoadmapWorkspaceEvent(command.UserRoadmapId, command.EventType, command.GetMetadataJson(), lastVersion);
        await repository.AddAsync(addEvent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        // todo: publish event to bus
    }
}
