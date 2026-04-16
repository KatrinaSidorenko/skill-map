using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItemConnection;

public record DeleteLearningItemConnectionCommand(long WorkspaceId, string Id, int BaseVersion, string IdempotencyKey) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.ConnectionDeleted;
    public object GetMetadata() => new LearningItemConnectionDeletedEvent(Id);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
    public RoadmapWorkspaceEvent ToRoadmapWorkspaceEvent()
        => new(WorkspaceId, EventType, GetMetadataJson(), BaseVersion + 1, IdempotencyKey);
}