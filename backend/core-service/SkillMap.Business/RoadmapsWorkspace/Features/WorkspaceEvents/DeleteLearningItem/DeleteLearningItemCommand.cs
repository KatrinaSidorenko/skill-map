using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItem;

public record DeleteLearningItemCommand(long WorkspaceId, string Id, int ClientWorkspaceVersion, string IdempotencyKey) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.DeleteLearningItem;
    public object GetMetadata() => new LearningItemDeletedEvent(Id);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
    public RoadmapWorkspaceEvent ToRoadmapWorkspaceEvent(int version)
        => new(WorkspaceId, EventType, GetMetadataJson(), version, IdempotencyKey);

    public CreateLearningItemProjectionCommand GetItemStatusProjectionCommand()
    {
        var projectionDto = new CreateLearningItemProjectionDto(Id, false, null);
        return CreateLearningItemProjectionCommand.Create(WorkspaceId, new List<CreateLearningItemProjectionDto> { projectionDto });
    }
}