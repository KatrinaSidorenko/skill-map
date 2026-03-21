
using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot.Blueprint;

[UsedImplicitly]
internal sealed class BuildBlueprintWorkspaceSnapshotHandler(
    IRoadmapBlueprintRepository roadmapBlueprintRepository, 
    IRepository<RoadmapWorkspaceSnapshot> snapshotsRepository,
    IRepository<RoadmapWorkspaceEvent> eventsRepository,
    ILogger<BuildBlueprintWorkspaceSnapshotHandler> logger) : IRequestHandler<BuildBlueprintWorkspaceSnapshotCommand, long>
{
    public async Task<long> Handle(BuildBlueprintWorkspaceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var latestSnapshots = await snapshotsRepository.GetAllAsync(
            filter: s => s.RoadmapWorkspaceId == request.WorkspaceId,
            orderBy: q => q.OrderByDescending(s => s.CreatedAt).ThenByDescending(s => s.UpdatedAt),
            count: 1,
            ct: cancellationToken);
        var latestSnapshot = latestSnapshots.FirstOrDefault();
        if (latestSnapshot == null)
        {
            var roadmapBlueprintResult = await roadmapBlueprintRepository.GetRoadmapById(request.RoadmapId, cancellationToken);
            if (roadmapBlueprintResult.IsFailed)
            {
                logger.LogError("Roadmap blueprint with id {RoadmapId} not found", request.RoadmapId);
                throw new InvalidOperationException($"Roadmap blueprint with id {request.RoadmapId} not found");
            }
            var roadmapBlueprint = roadmapBlueprintResult.Data;
            var roadmapSnapshot = roadmapBlueprint.MakeRoadmapSnapshot();
            var initialSnapshot = new RoadmapWorkspaceSnapshot(request.WorkspaceId, null, RoadmapWorkspaceConstants.InitialVersion);
            await initialSnapshot.SetRoadmapSnapshot(roadmapSnapshot, cancellationToken);
            await snapshotsRepository.AddAsync(initialSnapshot, cancellationToken);
            await snapshotsRepository.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created initial snapshot for workspace {WorkspaceId} with version {Version}", request.WorkspaceId, RoadmapWorkspaceConstants.InitialVersion);
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
        newSnapshot.SetVersion(eventsList.OrderByDescending(e => e.Version).Single().Version);
        // todo: calculate progress and status
        newSnapshot.SetMetadata(new RoadmapSnapshotMetadata(0.1, Core.Constants.LearningStatus.InProgress));
        await newSnapshot.SetRoadmapSnapshot(newRoadmapSnapshot, cancellationToken);
        await snapshotsRepository.AddAsync(newSnapshot, cancellationToken);
        await snapshotsRepository.SaveChangesAsync(cancellationToken);
        return newSnapshot.Id;
    }
}
