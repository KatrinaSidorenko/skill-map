
using JetBrains.Annotations;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;

[UsedImplicitly]
internal sealed class BuildAuthorWorkspaceSnapshotHandler(
    IRepository<RoadmapWorkspaceSnapshot> snapshotsRepository,
    IRepository<RoadmapWorkspaceEvent> eventsRepository, 
    ILogger<BuildAuthorWorkspaceSnapshotHandler> logger) : IRequestHandler<BuildAuthorWorkspaceSnapshotCommand, long>
{
    private const int _initialVersion = 0;
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
            filter: e => e.RoadmapWorkspaceId == request.WorkspaceId && e.Version > latestSnapshot.Version, // todo: be carefull
            orderBy: q => q.OrderBy(e => e.CreatedAt),
            ct: cancellationToken);
        if (events.Count() == 0)
        {
            logger.LogInformation("No new events found for workspace {WorkspaceId} since last snapshot version {Version}. Returning existing snapshot.", request.WorkspaceId, latestSnapshot.Version);
            return latestSnapshot.Id;
        }

        // i need to merge the obtained from snapshot roadmap with the events
        // venets can be of diffrent types and it should be clear to apply to the roadmap
        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(cancellationToken);

        // maybe here is the pattern to witch i can give the roadmap and feed the events and it will iteratevly apply them??
        var newSnapshot = new RoadmapWorkspaceSnapshot(request.WorkspaceId);
        newSnapshot.IncrementVersion(latestSnapshot.Version);
        newSnapshot.SetRoadmapSnapshot(null, cancellationToken);
        await snapshotsRepository.AddAsync(newSnapshot, cancellationToken);
        await snapshotsRepository.SaveChangesAsync(cancellationToken);
        return newSnapshot.Id;
    }
}
