using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;

[UsedImplicitly]
internal sealed class GetRoadmapWorkspacesHandler(IRepository<RoadmapWorkspace> repository) 
    : IRequestHandler<GetRoadmapWorkspacesQuery, PaginationResult<RoadmapWorkspaceSummaryDto>>
{
    public async Task<PaginationResult<RoadmapWorkspaceSummaryDto>> Handle(GetRoadmapWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var userWorkspaces = await repository.GetAllAsync(
            filter: rw => rw.AuthorId == request.UserId && rw.IsActive && !rw.IsInAuthorMode,
            pageNum: request.FilteringParams.PaginationParams.PageNumber,
            count: request.FilteringParams.PaginationParams.PageSize,
            ct: cancellationToken);

        // todo: it is not best solution to use latest snapshot, but wor now it is an optimal one. Than we need to add checks on new events
        var result = new List<RoadmapWorkspaceSummaryDto>();
        foreach (var workspace in userWorkspaces)
        {
            var latestSnapshot = workspace.Snapshots.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
            var snapshotMetadata = latestSnapshot?.ParseMetadata();
            if (snapshotMetadata == null) { continue; }

            //todo: how i can get plain info of roadmap like title, description and image url??
            result.Add(RoadmapWorkspaceSummaryDto.Create(
                workspace.Id,
                title: workspace.Metadata?.Title ?? string.Empty,
                description: workspace.Metadata?.Description ?? string.Empty,
                imageUrl: workspace.Metadata?.ImageUrl ?? string.Empty,
                latestSnapshot.CreatedAt,
                snapshotMetadata?.Status,
                snapshotMetadata?.Progress));
        }

        return new PaginationResult<RoadmapWorkspaceSummaryDto>
        {
            Result = result,
            //TotalCount = totalCount
        };
    }
}
