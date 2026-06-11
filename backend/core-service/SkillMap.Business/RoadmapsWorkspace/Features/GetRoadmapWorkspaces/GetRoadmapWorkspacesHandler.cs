using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.Helpers;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;

[UsedImplicitly]
internal sealed class GetRoadmapWorkspacesHandler(IRoadmapWorkspaceRepository repository, IRoadmapWorkspaceImageService roadmapWorkspaceImageService) : IRequestHandler<GetRoadmapWorkspacesQuery, PaginationResult<RoadmapWorkspaceSummaryDto>>
{
    public async Task<PaginationResult<RoadmapWorkspaceSummaryDto>> Handle(GetRoadmapWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var userWorkspaces = await repository.GetUserActiveNonAuthoredWorkspacesWithProjections(
            request.UserId,
            request.FilteringParams.PaginationParams.PageNumber,
            request.FilteringParams.PaginationParams.PageSize,
            filter: !string.IsNullOrEmpty(request.FilteringParams.SearchTerm)
                ? w => w.Title.ToLower().Contains(request.FilteringParams.SearchTerm.ToLower())
                : null,
            orderBy: w => w.OrderByDescending(workspace => workspace.CreatedAt),
            cancellationToken);

        var result = new List<RoadmapWorkspaceSummaryDto>();
        foreach (var workspace in userWorkspaces)
        {
            var totalItems = workspace.LearningItemProjections.Count;
            var completedItems = workspace.LearningItemProjections.Count(p => p.Status == LearningStatus.Completed.ToStatusString());
            var (progress, status) = RoadmapWorkspaceSnapshotExtensions.CalculateSnapshotMetadata(totalItems, completedItems);
            var imageUrl = await roadmapWorkspaceImageService.GetImageAbsoluteUriSafeAsync(workspace.ImageUrl, cancellationToken);

            result.Add(RoadmapWorkspaceSummaryDto.Create(
                workspace.Id,
                title: workspace.Title,
                description: workspace.Description ?? string.Empty,
                imageUrl: imageUrl,
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