using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprintSummary;

[UsedImplicitly]
internal sealed class GetRoadmapBlueprintsSummaryHandler(IRoadmapBlueprintRepository repository, IRoadmapWorkspaceRepository roadmapWorkspaceRepository) : IRequestHandler<GetRoadmapBlueprintSummaryQuery, PaginationResult<RoadmapBlueprintSummaryDto>>
{
    public async Task<PaginationResult<RoadmapBlueprintSummaryDto>> Handle(GetRoadmapBlueprintSummaryQuery request, CancellationToken cancellationToken)
    {
        // todo: exceptions handling with such repositories
        var result = await repository.GetPlainRoadmaps(request.FilteringParams, cancellationToken);
        var blueprintBasedWorkspaces = await roadmapWorkspaceRepository.GetUserBlueprintWorkspaces(request.UserId, cancellationToken);
        var blueprintBasedWorkspacesIds = blueprintBasedWorkspaces.Select(x => x.RoadmapId).ToHashSet();

        return new PaginationResult<RoadmapBlueprintSummaryDto>
        {
            Result = result.Data.Result.Select(x => RoadmapBlueprintSummaryDto.Create(x, blueprintBasedWorkspacesIds.Contains(x.Id))).ToList(),
            TotalCount = result.Data.TotalCount
        };
    }
}
