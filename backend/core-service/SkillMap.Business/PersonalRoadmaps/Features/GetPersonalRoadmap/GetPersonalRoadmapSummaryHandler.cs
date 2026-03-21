using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.Roadmaps;

namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmap;

[UsedImplicitly]
internal sealed class GetPersonalRoadmapSummaryHandler(IRepository<PersonalRoadmap> repository) : IRequestHandler<GetPersonalRoadmapSummaryQuery, PersonalRoadmapSummaryDto>
{
    public async Task<PersonalRoadmapSummaryDto> Handle(GetPersonalRoadmapSummaryQuery request, CancellationToken cancellationToken)
    {
        var personalRoadmap = await repository.GetByIdAsync(request.GetPersonalRoadmapId(), cancellationToken);
        if (personalRoadmap is null || personalRoadmap.AuthorId != request.UserId)
        {
            throw new KeyNotFoundException($"Personal roadmap with id {request.PersonalRoadmapId} not found");
        }

        return PersonalRoadmapSummaryDto.Create(personalRoadmap);
    }
}
