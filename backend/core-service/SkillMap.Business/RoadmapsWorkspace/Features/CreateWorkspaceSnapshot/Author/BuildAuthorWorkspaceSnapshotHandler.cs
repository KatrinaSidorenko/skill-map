
using JetBrains.Annotations;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot.Author;

[UsedImplicitly]
internal sealed class BuildAuthorWorkspaceSnapshotHandler(
    IRoadmapWorkspaceSnapshotRepository snapshotsRepository,
    IRoadmapWorkspaceEventRepository eventsRepository,
    ILogger<BuildAuthorWorkspaceSnapshotHandler> logger) : IRequestHandler<BuildAuthorWorkspaceSnapshotCommand, long>
{
    private const int _initialVersion = RoadmapWorkspaceConstants.InitialVersion;
    public async Task<long> Handle(BuildAuthorWorkspaceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var latestSnapshot = await snapshotsRepository.GetLatestSnapshot(request.WorkspaceId, cancellationToken);
        if (latestSnapshot == null)
        {
            var initialSnapshot = new RoadmapWorkspaceSnapshot(request.WorkspaceId, null, _initialVersion);
            await snapshotsRepository.AddAsync(initialSnapshot, cancellationToken);
            await snapshotsRepository.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created initial snapshot for workspace {WorkspaceId} with version {Version}", request.WorkspaceId, _initialVersion);
            return initialSnapshot.Id;
        }

        var eventsList = await eventsRepository.GetEventsGreaterThan(request.WorkspaceId, latestSnapshot.Version, cancellationToken);
        if (eventsList.Count <= 0)
        {
            logger.LogInformation("No new events found for workspace {WorkspaceId} since last snapshot version {Version}. Returning existing snapshot.", request.WorkspaceId, latestSnapshot.Version);
            return latestSnapshot.Id;
        }

        var newWorkspaceSnapshot = await RoadmapWorkspaceSnapshotExtensions.CreateRoadmapWorkspaceSnapshot(
            request.WorkspaceId,
            latestSnapshot,
            eventsList,
            cancellationToken);
        await snapshotsRepository.AddAsync(newWorkspaceSnapshot, cancellationToken);
        await snapshotsRepository.SaveChangesAsync(cancellationToken);
        return newWorkspaceSnapshot.Id;
    }
}
