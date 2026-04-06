
using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot.Blueprint;

[UsedImplicitly]
internal sealed class BuildBlueprintWorkspaceSnapshotHandler(
    IRoadmapBlueprintRepository roadmapBlueprintRepository,
    IRoadmapWorkspaceSnapshotRepository snapshotsRepository,
    IRoadmapWorkspaceEventRepository eventsRepository,
    ILogger<BuildBlueprintWorkspaceSnapshotHandler> logger) : IRequestHandler<BuildBlueprintWorkspaceSnapshotCommand, long>
{
    public async Task<long> Handle(BuildBlueprintWorkspaceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var latestSnapshot = await snapshotsRepository.GetLatestSnapshot(request.WorkspaceId, cancellationToken);
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

        var eventsList = await eventsRepository.GetCheckedEventsGreaterThan(request.WorkspaceId, latestSnapshot.Version, cancellationToken);
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