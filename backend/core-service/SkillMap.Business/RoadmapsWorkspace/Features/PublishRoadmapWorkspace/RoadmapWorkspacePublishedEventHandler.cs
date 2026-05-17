using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.PublishRoadmapWorkspace;

[UsedImplicitly]
internal sealed class RoadmapWorkspacePublishedEventHandler(
    IRoadmapWorkspaceSnapshotRepository snapshotsRepository,
    IRoadmapWorkspaceEventRepository eventsRepository,
    IRoadmapBlueprintRepository roadmapBlueprintRepository,
    IRepository<RoadmapWorkspace> workspaceRepository,
    ILogger<RoadmapWorkspacePublishedEventHandler> logger) : IIntegrationEventHandler<PersonalRoadmapPublishedEvent>
{
    public async Task Handle(PersonalRoadmapPublishedEvent notification, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetFirstOrDefaultAsync(w => w.Id == notification.WorkspaceId && w.IsInAuthorMode, cancellationToken)
            ?? throw new InvalidOperationException($"No found workspace {notification.WorkspaceId}");

        var latestSnapshot = await snapshotsRepository.GetLatestSnapshot(notification.WorkspaceId, cancellationToken)
            ?? throw new InvalidOperationException($"No snapshot found for workspace {notification.WorkspaceId}");

        var eventsList = await eventsRepository.GetEventsAfter(notification.WorkspaceId, latestSnapshot.Version, cancellationToken);
        if (eventsList.Count <= 0)
        {
            logger.LogInformation("No new events found for workspace {WorkspaceId} since last snapshot version {Version}. Returning existing snapshot.", notification.WorkspaceId, latestSnapshot.Version);
            return;
        }

        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(cancellationToken);
        var newRoadmapSnapshot = await snapshotContent.ApplyEventsToSnapshot(eventsList, cancellationToken);
        var blueprint = newRoadmapSnapshot.ToCreateBlueprint(
            title: workspace.Title,
            description: workspace.Description,
            imageUrl: workspace.ImageUrl,
            authorId: notification.AuthorId,
            version: eventsList.OrderByDescending(e => e.Version).First().Version);
        await roadmapBlueprintRepository.CreateFullRoadmap(blueprint, cancellationToken);
    }
}