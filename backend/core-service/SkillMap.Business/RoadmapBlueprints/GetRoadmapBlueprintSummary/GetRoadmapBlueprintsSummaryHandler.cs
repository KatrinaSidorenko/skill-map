using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprintSummary;

[UsedImplicitly]
internal sealed class GetRoadmapBlueprintsSummaryHandler(IRoadmapBlueprintRepository repository) : IRequestHandler<GetRoadmapBlueprintSummaryQuery, PaginationResult<RoadmapBlueprintSummaryDto>>
{
    public async Task<PaginationResult<RoadmapBlueprintSummaryDto>> Handle(GetRoadmapBlueprintSummaryQuery request, CancellationToken cancellationToken)
    {
        // todo: exceptions handling with such repositories
        var result = await repository.GetPlainRoadmaps(request.FilteringParams, cancellationToken);
        return new PaginationResult<RoadmapBlueprintSummaryDto>
        {
            Result = result.Data.Result.Select(x => RoadmapBlueprintSummaryDto.Create(x)).ToList(),
            TotalCount = result.Data.TotalCount
        };
    }
}
