using System;

using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
using SkillMap.Business.RoadmapBookmarks.IntegrationEvents;
using SkillMap.Core.Constants;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.ForkRoadmap;

[UsedImplicitly]
internal sealed class CreateRoadmapWorkspaceHandler(IRoadmapService roadmapService) : IIntegrationEventHandler<RoadmapBookmarkAddedEvent>
{
    public async Task Handle(RoadmapBookmarkAddedEvent notification, CancellationToken cancellationToken)
    {
        // make initial snapshots
        // than we will write to WAL
        // and based on snapshot and WAL we will create workspace for user and roadmap
    }
}
