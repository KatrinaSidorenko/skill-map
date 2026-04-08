using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;

public record CreateLearningItemConnectionCommand(long WorkspaceId, string Id, string Source, string Target, int ClientWorkspaceVersion, string IdempotencyKey) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.CreateConnection;
    public object GetMetadata() => new LearningItemConnectionCreatedEvent(Id, Source, Target);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
    public RoadmapWorkspaceEvent ToRoadmapWorkspaceEvent(int version)
        => new(WorkspaceId, EventType, GetMetadataJson(), version, IdempotencyKey);
}