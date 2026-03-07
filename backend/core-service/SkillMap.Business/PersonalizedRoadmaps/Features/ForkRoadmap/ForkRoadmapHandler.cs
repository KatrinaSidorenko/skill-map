using LearningPlatform.Roadmap.Business.Contracts;

using SkillMap.Business.RoadmapBookmarks.IntegrationEvents;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.ForkRoadmap;
public class ForkRoadmapHandler(IRoadmapService roadmapService) : IIntegrationEventHandler<RoadmapBookmarkAddedEvent>
{
    public async Task Handle(RoadmapBookmarkAddedEvent notification, CancellationToken cancellationToken)
    {
       
    }
}
