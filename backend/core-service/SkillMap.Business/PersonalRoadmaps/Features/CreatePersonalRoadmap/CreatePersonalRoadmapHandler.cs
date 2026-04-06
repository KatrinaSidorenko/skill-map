
using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
using SkillMap.Core.Roadmaps;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalRoadmaps.Features.CreatePersonalRoadmap;

[UsedImplicitly]
internal sealed class CreatePersonalRoadmapHandler(IRepository<PersonalRoadmap> repository, IEventBus eventBus) : IRequestHandler<CreatePersonalRoadmapCommand, long>
{
    public async Task<long> Handle(CreatePersonalRoadmapCommand request, CancellationToken cancellationToken)
    {
        var personalRoadmap = new PersonalRoadmap(
            request.UserId,
            request.Title,
            request.Description,
            request.ImageUrl);
        await repository.AddAsync(personalRoadmap, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventBus.PublishAsync(PersonalRoadmapCreatedEvent.Create(
            personalRoadmap.AuthorId,
            personalRoadmap.Id,
            request.Title,
            request.Description,
            request.ImageUrl), cancellationToken);

        return personalRoadmap.Id;
    }
}