
using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;

[UsedImplicitly]
internal sealed class RoadmapWorkspaceChangedEventHandler(
    IRoadmapWorkspaceSnapshotRepository snapshotsRepository,
    IRoadmapWorkspaceEventRepository eventsRepository,
    ILogger<RoadmapWorkspaceChangedEventHandler> logger) : IIntegrationEventHandler<RoadmapWorkspaceChangedEvent>
{

    // todo: better extract to inbox_tasks logic
    public async Task Handle(RoadmapWorkspaceChangedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.NewVersion % RoadmapWorkspaceConstants.SnapshotCreationInterval != 0)
        {
            logger.LogInformation("Received RoadmapWorkspaceChangedEvent for workspace {WorkspaceId} with version {Version}, which is not a snapshot creation point. Skipping snapshot creation.", notification.WorkspaceId, notification.NewVersion);
            return;
        }

        var latestSnapshot = await snapshotsRepository.GetLatestSnapshot(notification.WorkspaceId, cancellationToken)
            ?? throw new InvalidOperationException($"No snapshot found for workspace {notification.WorkspaceId}");

        var eventsList = await eventsRepository.GetEventsGreaterThan(notification.WorkspaceId, latestSnapshot.Version, cancellationToken);
        if (eventsList.Count <= 0)
        {
            logger.LogInformation("No new events found for workspace {WorkspaceId} since last snapshot version {Version}. Returning existing snapshot.", notification.WorkspaceId, latestSnapshot.Version);
            return;
        }

        var newWorkspaceSnapshot = await RoadmapWorkspaceSnapshotExtensions.CreateRoadmapWorkspaceSnapshot(
           notification.WorkspaceId,
           latestSnapshot,
           eventsList,
           cancellationToken);
        await snapshotsRepository.AddAsync(newWorkspaceSnapshot, cancellationToken);
        await snapshotsRepository.SaveChangesAsync(cancellationToken);
    }
}
