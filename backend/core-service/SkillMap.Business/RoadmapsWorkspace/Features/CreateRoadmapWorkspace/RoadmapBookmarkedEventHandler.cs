using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
using SkillMap.Business.RoadmapBookmarks.IntegrationEvents;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.ForkRoadmap;

[UsedImplicitly]
internal sealed class RoadmapBookmarkedEventHandler(IRoadmapService roadmapService, IRepository<RoadmapWorkspaceSnapshot> repository) : IIntegrationEventHandler<RoadmapBookmarkAddedEvent>
{
    private const int InitialVersion = 1;
    public async Task Handle(RoadmapBookmarkAddedEvent notification, CancellationToken cancellationToken)
    {
        // make initial snapshots
        // than we will write to WAL
        // and based on snapshot and WAL we will create workspace for user and roadmap

        // we can have source roadmap or not have
        // for now let's work if not
        var workspaceSnapshot = (RoadmapWorkspaceSnapshot)null;
        if (!notification.IsSourceRoadmapAvailable)
        {
            // create an empty workspace snapshot for bookmark
            workspaceSnapshot = new RoadmapWorkspaceSnapshot(notification.BookmarkId, null, InitialVersion);
        }

        // if roadmap is available - create snapshot based on roadmap

        await repository.AddAsync(workspaceSnapshot, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
