using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaceSummary;

[UsedImplicitly]
internal sealed class GetRoadmapWorkspaceSummaryHandler(IRepository<RoadmapWorkspace> repository, IRoadmapLearningItemStatusRepository roadmapLearningItemStatusRepository)
    : IRequestHandler<GetRoadmapWorkspaceSummaryQuery, RoadmapWorkspaceSummaryDto>
{
    public async Task<RoadmapWorkspaceSummaryDto> Handle(GetRoadmapWorkspaceSummaryQuery request, CancellationToken cancellationToken)
    {
        var workspace = await repository.GetFirstOrDefaultAsync(
           w => w.Id == request.WorkspaceId && w.IsActive, cancellationToken)
             ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), request.WorkspaceId.ToString());

        var latestSnapshot = workspace.Snapshots.OrderByDescending(s => s.CreatedAt).FirstOrDefault()
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspaceSnapshot), $"No snapshots found for workspace {request.WorkspaceId}");

        var (totalItems, completedItems) = await roadmapLearningItemStatusRepository.GetWorkspaceProgressAsync(request.WorkspaceId, cancellationToken);
        var snapshotMetadata = RoadmapWorkspaceSnapshotExtensions.CalculateSnapshotMetadata(totalItems, completedItems);

        return RoadmapWorkspaceSummaryDto.Create(
            workspace.Id,
            title: workspace.Metadata?.Title ?? string.Empty,
            description: workspace.Metadata?.Description ?? string.Empty,
            imageUrl: workspace.Metadata?.ImageUrl ?? string.Empty,
            workspace.CreatedAt,
            snapshotMetadata?.Status,
            snapshotMetadata?.Progress);
    }
}