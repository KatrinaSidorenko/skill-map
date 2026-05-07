using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot.Blueprint;

[UsedImplicitly]
internal sealed class BuildBlueprintWorkspaceSnapshotHandler(
    IRoadmapBlueprintRepository roadmapBlueprintRepository,
    IRoadmapWorkspaceSnapshotRepository snapshotsRepository,
    IRoadmapWorkspaceEventRepository eventsRepository,
    IEventBus eventBus,
    ILogger<BuildBlueprintWorkspaceSnapshotHandler> logger) : IRequestHandler<BuildBlueprintWorkspaceSnapshotCommand, long>
{
    public async Task<long> Handle(BuildBlueprintWorkspaceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var latestSnapshot = await snapshotsRepository.GetLatestSnapshot(request.WorkspaceId, cancellationToken);
        if (latestSnapshot == null)
        {
            var initialVersion = RoadmapWorkspaceConstants.InitialVersion;
            var initialSnapshot = new RoadmapWorkspaceSnapshot(request.WorkspaceId, null, initialVersion);
            if (string.IsNullOrEmpty(request.RoadmapId))
            {
                initialSnapshot = new RoadmapWorkspaceSnapshot(request.WorkspaceId, null, initialVersion);
                await initialSnapshot.SetRoadmapSnapshot(RoadmapWorkspaceSnapshotExtensions.GetEmptyRoadmapSnapshot(request.RoadmapId), cancellationToken);
                await snapshotsRepository.AddAsync(initialSnapshot, cancellationToken);
                await snapshotsRepository.SaveChangesAsync(cancellationToken);
                return initialSnapshot.Id;
            }
            var roadmapBlueprintResult = await roadmapBlueprintRepository.GetRoadmapById(request.RoadmapId, cancellationToken);
            if (roadmapBlueprintResult.IsFailed)
            {
                logger.LogError("Roadmap blueprint with id {RoadmapId} not found", request.RoadmapId);
                throw new InvalidOperationException($"Roadmap blueprint with id {request.RoadmapId} not found");
            }

            var roadmapBlueprint = roadmapBlueprintResult.Data;
            var roadmapSnapshot = roadmapBlueprint.MakeRoadmapSnapshot().EnsureNoCycleEdges(logger);

            await initialSnapshot.SetRoadmapSnapshot(roadmapSnapshot, cancellationToken);
            await snapshotsRepository.AddAsync(initialSnapshot, cancellationToken);
            await snapshotsRepository.SaveChangesAsync(cancellationToken);

            var learningItemsProjections = roadmapSnapshot.LearningItems.Select(i => CreateLearningItemProjectionDto.ToProjectionDto(i)).ToList();
            var createProjectionsCommand = CreateLearningItemProjectionCommand.Create(request.WorkspaceId, learningItemsProjections);
            await eventBus.PublishAsync(createProjectionsCommand, cancellationToken);

            logger.LogInformation(
              "Created initial snapshot for workspace {WorkspaceId} with version {Version}",
                   request.WorkspaceId, initialVersion);

            return initialSnapshot.Id;
        }

        var eventsList = await eventsRepository.GetEventsAfter(request.WorkspaceId, latestSnapshot.Version, cancellationToken);
        if (eventsList.Count <= 0)
        {
            logger.LogInformation(
                "No new events found for workspace {WorkspaceId} since last snapshot version {Version}. Returning existing snapshot.",
                request.WorkspaceId, latestSnapshot.Version);

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