using MimeKit.Cryptography;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;

public class RoadmapWorkspaceEditor(
    IRepository<RoadmapWorkspaceSnapshot> snapshotsRepository,
    IRoadmapWorkspaceRepository workspaceRepository,
    IRoadmapWorkspaceEventRepository eventsRepository) : IRoadmapWorkspaceEditor
{

    private async Task<RoadmapWorkspaceSnapshot> GetLatestSnapshot(long workspaceId, CancellationToken cancellationToken)
    {
        var latestSnapshot = (await snapshotsRepository.GetAllAsync(s => s.RoadmapWorkspaceId == workspaceId,
            orderBy: q => q.OrderByDescending(w => w.CreatedAt),
            count: 1,
            ct: cancellationToken)).FirstOrDefault()
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspaceSnapshot), $"No snapshots found for workspace {workspaceId}");
        return latestSnapshot;
    }
    public async Task<RoadmapSnapshot> GetActualRoadmapSnapshot(long workspaceId, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetFirstOrDefaultAsync(w => w.Id == workspaceId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), workspaceId.ToString());
        var latestSnapshot = await GetLatestSnapshot(workspaceId, cancellationToken);
        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(cancellationToken);
        var events = await eventsRepository.GetEventsAfter(workspace.Id, latestSnapshot.Version, cancellationToken);

        var targetSnapshotContent = snapshotContent;
        if (events.Count > 0)
        {
            targetSnapshotContent = await snapshotContent.ApplyEventsToSnapshot(events, cancellationToken);
        }

        targetSnapshotContent.Version = events.Count > 0 ? events.Max(e => e.Version) : latestSnapshot.Version;
        targetSnapshotContent.LearningItems = targetSnapshotContent.LearningItems.DistinctBy(li => li.Id).ToList();
        targetSnapshotContent.LearningItemsConnections = targetSnapshotContent.LearningItemsConnections.DistinctBy(c => c.Id).ToList();
        return targetSnapshotContent;
    }
}