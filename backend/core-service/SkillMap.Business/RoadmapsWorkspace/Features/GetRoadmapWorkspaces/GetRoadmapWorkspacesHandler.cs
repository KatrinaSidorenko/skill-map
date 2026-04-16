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
internal sealed class GetRoadmapWorkspacesHandler(IRoadmapWorkspaceRepository repository, IRoadmapLearningItemProjectionRepository roadmapLearningItemStatusRepository)
    : IRequestHandler<GetRoadmapWorkspacesQuery, PaginationResult<RoadmapWorkspaceSummaryDto>>
{
    public async Task<PaginationResult<RoadmapWorkspaceSummaryDto>> Handle(GetRoadmapWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var userWorkspaces = await repository.GetUserActiveNonAuthoredWorkspacesWithSnapshots(
            request.UserId,
            request.FilteringParams.PaginationParams.PageNumber,
            request.FilteringParams.PaginationParams.PageSize,
            cancellationToken);

        var result = new List<RoadmapWorkspaceSummaryDto>();
        foreach (var workspace in userWorkspaces)
        {
            var totalItems = workspace.LearningItemProjections.Count;
            var completedItems = workspace.LearningItemProjections.Count(p => p.Status == LearningStatus.Completed.ToStatusString());
            var (progress, status) = RoadmapWorkspaceSnapshotExtensions.CalculateSnapshotMetadata(totalItems, completedItems);
            if (workspace.Metadata == null) { continue; }

            result.Add(RoadmapWorkspaceSummaryDto.Create(
                workspace.Id,
                title: workspace.Metadata?.Title ?? string.Empty,
                description: workspace.Metadata?.Description ?? string.Empty,
                imageUrl: workspace.Metadata?.ImageUrl ?? string.Empty,
                workspace.CreatedAt,
                status,
                progress));
        }

        return new PaginationResult<RoadmapWorkspaceSummaryDto>
        {
            Result = result,
            //TotalCount = totalCount
        };
    }
}