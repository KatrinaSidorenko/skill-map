using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.Roadmaps;
using SkillMap.Shared.Models;

namespace SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmaps;

[UsedImplicitly]
internal sealed class GetPersonalRoadmapsHandler(IRepository<PersonalRoadmap> repository) : IRequestHandler<GetPersonalRoadmapsQuery, PaginationResult<PersonalRoadmapDto>>
{
    public async Task<PaginationResult<PersonalRoadmapDto>> Handle(GetPersonalRoadmapsQuery request, CancellationToken cancellationToken)
    {
        var personalRoadmaps = await repository.GetAllAsync(
            filter: pr => pr.AuthorId == request.UserId,
            pageNum: request.FilteringParams.PaginationParams.PageNumber,
            count: request.FilteringParams.PaginationParams.PageSize,
            ct: cancellationToken);

        //var totalCount = await repository.(pr => pr.AuthorId == request.UserId, cancellationToken);
        return new PaginationResult<PersonalRoadmapDto>()
        {
            Result = personalRoadmaps.Select(p => PersonalRoadmapDto.Create(p)).ToList(),
            //TotalCount = personalRoadmaps.Count
        };
    }
}