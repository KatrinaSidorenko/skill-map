using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.Roadmaps;
using SkillMap.Shared.Models;

namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmaps;

[UsedImplicitly]
internal sealed class GetPersonalRoadmapsHandler(IRepository<PersonalRoadmap> repository) : IRequestHandler<GetPersonalRoadmapsQuery, PaginationResult<PersonalRoadmapsDto>>
{
    public async Task<PaginationResult<PersonalRoadmapsDto>> Handle(GetPersonalRoadmapsQuery request, CancellationToken cancellationToken)
    {
        var personalRoadmaps = await repository.GetAllAsync(
            filter: pr => pr.AuthorId == request.UserId,
            pageNum: request.PaginationParams.PageNumber,
            count: request.PaginationParams.PageSize,
            ct: cancellationToken);

        //var totalCount = await repository.(pr => pr.AuthorId == request.UserId, cancellationToken);
        return new PaginationResult<PersonalRoadmapsDto>()
        {
            Result = PersonalRoadmapsDto.Create(personalRoadmaps),
            //TotalCount = personalRoadmaps.Count
        };
    }
}
