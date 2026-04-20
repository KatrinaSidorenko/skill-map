using System;

using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaceSummary;

[UsedImplicitly]
internal sealed class GetRoadmapWorkspaceSummaryHandler(IRoadmapWorkspaceRepository repository)
    : IRequestHandler<GetRoadmapWorkspaceSummaryQuery, RoadmapWorkspaceSummaryDto>
{
    public async Task<RoadmapWorkspaceSummaryDto> Handle(GetRoadmapWorkspaceSummaryQuery request, CancellationToken cancellationToken)
    {
        var workspace = await repository.GetUserActiveWorkspace(request.WorkspaceId, cancellationToken)
             ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), request.WorkspaceId.ToString());

        var totalItems = workspace.LearningItemProjections.Count;
        var completedItems = workspace.LearningItemProjections.Count(p => p.Status == LearningStatus.Completed.ToStatusString());
        var (progress, status) = RoadmapWorkspaceSnapshotExtensions.CalculateSnapshotMetadata(totalItems, completedItems);

        return RoadmapWorkspaceSummaryDto.Create(
            workspace.Id,
            title: workspace.Metadata?.Title ?? string.Empty,
            description: workspace.Metadata?.Description ?? string.Empty,
            imageUrl: workspace.Metadata?.ImageUrl ?? string.Empty,
            workspace.CreatedAt,
            status,
            progress);
    }
}