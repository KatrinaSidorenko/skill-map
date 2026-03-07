using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SkillMap.Shared.EventBus;

namespace SkillMap.Business.PersonalRoadmaps.IntegrationEvents;
public record PersonalRoadmapCreatedEvent(Guid Id, long UserId, string RoadmapId, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static PersonalRoadmapCreatedEvent Create(long userId, long roadmapId)
        => new (Guid.NewGuid(), userId, roadmapId.ToString(), DateTimeOffset.UtcNow);
}
