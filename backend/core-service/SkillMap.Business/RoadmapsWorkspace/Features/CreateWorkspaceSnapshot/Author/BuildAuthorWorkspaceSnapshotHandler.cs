
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
    IRepository<RoadmapWorkspaceSnapshot> snapshotsRepository,
    IRepository<RoadmapWorkspaceEvent> eventsRepository,
    ILogger<BuildAuthorWorkspaceSnapshotHandler> logger) : IRequestHandler<BuildAuthorWorkspaceSnapshotCommand, long>
{
    private const int _initialVersion = RoadmapWorkspaceConstants.InitialVersion;
    public async Task<long> Handle(BuildAuthorWorkspaceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var latestSnapshots = await snapshotsRepository.GetAllAsync(
            filter: s => s.RoadmapWorkspaceId == request.WorkspaceId,
            orderBy: q => q.OrderByDescending(s => s.CreatedAt).ThenByDescending(s => s.UpdatedAt),
            count: 1,
            ct: cancellationToken);
        var latestSnapshot = latestSnapshots.FirstOrDefault();
        if (latestSnapshot == null)
        {
            var initialSnapshot = new RoadmapWorkspaceSnapshot(request.WorkspaceId, null, _initialVersion);
            await snapshotsRepository.AddAsync(initialSnapshot, cancellationToken);
            await snapshotsRepository.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created initial snapshot for workspace {WorkspaceId} with version {Version}", request.WorkspaceId, _initialVersion);
            return initialSnapshot.Id;
        }

        var events = await eventsRepository.GetAllAsync(
            filter: e => e.RoadmapWorkspaceId == request.WorkspaceId && e.Version > latestSnapshot.Version,
            ct: cancellationToken);
        var eventsList = events.ToList();
        if (eventsList.Count <= 0)
        {
            logger.LogInformation("No new events found for workspace {WorkspaceId} since last snapshot version {Version}. Returning existing snapshot.", request.WorkspaceId, latestSnapshot.Version);
            return latestSnapshot.Id;
        }

        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(cancellationToken);
        var newRoadmapSnapshot = await snapshotContent.ApplyEventsToSnapshot(eventsList, cancellationToken);

        var newSnapshot = new RoadmapWorkspaceSnapshot(request.WorkspaceId);
        newSnapshot.SetVersion(eventsList.OrderByDescending(e => e.Version).First().Version);
        newSnapshot.SetMetadata(newRoadmapSnapshot.CalculateSnapshotMetadata()); 
        await newSnapshot.SetRoadmapSnapshot(newRoadmapSnapshot, cancellationToken);
        await snapshotsRepository.AddAsync(newSnapshot, cancellationToken);
        await snapshotsRepository.SaveChangesAsync(cancellationToken);
        return newSnapshot.Id;
    }
}
