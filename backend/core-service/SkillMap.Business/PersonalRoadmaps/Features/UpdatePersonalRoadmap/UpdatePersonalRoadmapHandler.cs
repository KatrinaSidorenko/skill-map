using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.Roadmaps;
using SkillMap.Shared.Results;

namespace SkillMap.Business.PersonalRoadmaps.Features.UpdatePersonalRoadmap;

[UsedImplicitly]
internal sealed class UpdatePersonalRoadmapHandler(IRepository<PersonalRoadmap> repository)
    : IRequestHandler<UpdatePersonalRoadmapCommand>
{
    public async Task Handle(UpdatePersonalRoadmapCommand request, CancellationToken cancellationToken)
    {
        var personalRoadmap = await repository.GetFirstOrDefaultAsync(
            r => r.Id == request.PersonalRoadmapId && r.AuthorId == request.UserId,
    ct: cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(PersonalRoadmap), request.PersonalRoadmapId.ToString());

        personalRoadmap.Title = request.Title;
        personalRoadmap.Description = request.Description;
        personalRoadmap.ImageUrl = request.ImageUrl;

        await repository.UpdateAsync(personalRoadmap, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}