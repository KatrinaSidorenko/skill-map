using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.AddLearningItem;

[UsedImplicitly]
internal sealed class AddLearningItemCommandHandler(IRepository<PersonalizeRoadmapEvent> repository) : IRequestHandler<AddLearningItemCommand>
{
    public async Task Handle(AddLearningItemCommand command, CancellationToken cancellationToken)
    {
        var addEvent = new PersonalizeRoadmapEvent(command.UserRoadmapId, command.EventType, command.GetMetadata());
        await repository.AddAsync(addEvent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
