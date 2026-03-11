using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;

[UsedImplicitly]
internal sealed class GetRoadmapWorkspacesHandler(
    IRepository<RoadmapWorkspace> repository,
    IRoadmapBlueprintRepository roadmapBlueprintRepository) : IRequestHandler<GetRoadmapWorkspacesQuery, PaginationResult<RoadmapWorkspaceSummaryDto>>
{
    public async Task<PaginationResult<RoadmapWorkspaceSummaryDto>> Handle(GetRoadmapWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var userWorkspaces = await repository.GetAllAsync(
            filter: rw => rw.AuthorId == request.UserId && rw.IsActive,
            pageNum: request.FilteringParams.PaginationParams.PageNumber,
            count: request.FilteringParams.PaginationParams.PageSize,
            ct: cancellationToken);

        // TODO: Calculate actual progress from the latest snapshot
        var progress = 0.0; // Mock value

        // TODO: Calculate actual status from the latest snapshot learning items
        var status = LearningStatus.NotStarted; // Mock value

        // for personal roadmps we want to get data by navigational properties.
        // for roadmaps with externalid we want to search for latest snapshot and get all data from it + in future there fill be the progress ans status
        // but for now it mock

        return new PaginationResult<RoadmapWorkspaceSummaryDto>
        {
            Result = result,
            //TotalCount = totalCount
        };
    }
}
