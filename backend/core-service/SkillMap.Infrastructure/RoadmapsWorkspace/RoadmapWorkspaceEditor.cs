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
    IRoadmapWorkspaceRepository workspaceRepository,
    IRoadmapWorkspaceEventRepository eventsRepository) : IRoadmapWorkspaceEditor
{
    public async Task<RoadmapSnapshot> GetActualRoadmapSnapshot(long workspaceId, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetFirstOrDefaultAsync(w => w.Id == workspaceId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), workspaceId.ToString());
        var latestSnapshot = workspace.Snapshots.OrderByDescending(s => s.CreatedAt).FirstOrDefault()
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspaceSnapshot), $"No snapshots found for workspace {workspaceId}");

        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(cancellationToken);
        var events = await eventsRepository.GetCheckedEventsGreaterThan(workspace.Id, latestSnapshot.Version, cancellationToken);
        var eventsList = events.ToList();

        var targetSnapshotContent = snapshotContent;
        if (eventsList.Count > 0)
        {
            targetSnapshotContent = await snapshotContent.ApplyEventsToSnapshot(eventsList, cancellationToken);
        }

        targetSnapshotContent.Version = eventsList.Count > 0 ? eventsList.Max(e => e.Version) : latestSnapshot.Version;
        targetSnapshotContent.LearningItems = targetSnapshotContent.LearningItems.DistinctBy(li => li.Id).ToList();
        targetSnapshotContent.LearningItemsConnections = targetSnapshotContent.LearningItemsConnections.DistinctBy(c => c.Id).ToList();

        return targetSnapshotContent;
    }
}