using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using SkillMap.Business.RoadmapBookmarks.IntegrationEvents;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.ForkRoadmap;

[UsedImplicitly]
internal sealed class ForkRoadmapHandler(IRoadmapService roadmapService) : IIntegrationEventHandler<RoadmapBookmarkAddedEvent>
{
    public async Task Handle(RoadmapBookmarkAddedEvent notification, CancellationToken cancellationToken)
    {
        // get the current roadmap
        // make a snapshot with 1 version
    }
}
