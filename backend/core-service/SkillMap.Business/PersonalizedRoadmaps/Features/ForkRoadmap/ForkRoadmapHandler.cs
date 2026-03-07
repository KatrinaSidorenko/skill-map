using LearningPlatform.Roadmap.Business.Contracts;

using SkillMap.Business.PersonalizedRoadmaps.IntegrationEvents;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.ForkRoadmap;
public class ForkRoadmapHandler(IRoadmapService roadmapService) : IIntegrationEventHandler<ForkRoadmapEvent>
{
    public async Task Handle(ForkRoadmapEvent notification, CancellationToken cancellationToken)
    {
       
    }
}
