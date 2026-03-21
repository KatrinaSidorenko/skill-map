using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
using SkillMap.Core.Roadmaps;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalRoadmaps.Features.PublishPersonalRoadmap;

[UsedImplicitly]
internal sealed class PublishPersonalRoadmapHandler(IRepository<PersonalRoadmap> repository, IEventBus eventBus) : IRequestHandler<PublishPersonalRoadmapCommand>
{
    public async Task Handle(PublishPersonalRoadmapCommand request, CancellationToken cancellationToken)
    {
        // get the latest roadmap workspace for the given roadmap id
        // and publish it by using blueprint repository
        var personalRoadmap = await repository.GetFirstOrDefaultAsync(
            roadmap => roadmap.Id == request.PersonalRoadmapId,
            ct: cancellationToken) ?? throw new InvalidOperationException($"Personal roadmap with id {request.PersonalRoadmapId} not found.");

        personalRoadmap.Publish();
        await repository.UpdateAsync(personalRoadmap, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        // todo: it is volatile the boundaries
        await eventBus.PublishAsync(PersonalRoadmapPublishedEvent.Create(personalRoadmap.RoadmapWorkspace.Id, personalRoadmap.AuthorId), cancellationToken);
    }
}
