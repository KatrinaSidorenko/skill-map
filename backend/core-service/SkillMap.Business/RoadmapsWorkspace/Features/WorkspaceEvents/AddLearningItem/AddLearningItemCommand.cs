using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.AddLearningItem;
public record AddLearningItemCommand(long WorkspaceId, string Id, string Title, string Description, string Status, string Type, int ClientWorkspaceVersion, string IdempotencyKey) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.CreateLearningItem;
    public object GetMetadata()
        => new LearningItemCreatedEvent(Id, Title, Description, Type, Status ?? LearningStatus.NotStarted.ToStatusString());
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
    public RoadmapWorkspaceEvent ToRoadmapWorkspaceEvent(int version)
        => new(WorkspaceId, EventType, GetMetadataJson(), version, IdempotencyKey);
}