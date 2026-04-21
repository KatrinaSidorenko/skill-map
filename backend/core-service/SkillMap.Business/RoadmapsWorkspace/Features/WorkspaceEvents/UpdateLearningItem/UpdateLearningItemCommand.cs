using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.UpdateLearningItem;

public record UpdateLearningItemCommand(long WorkspaceId, string Id, string? Title, string? Description, string? Status, string? Type, int BaseVersion, string IdempotencyKey) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.LearningItemUpdated;
    public object GetMetadata()
        => new LearningItemUpdatedEvent(Id, Title, Description, Status, Type);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
    public RoadmapWorkspaceEvent ToRoadmapWorkspaceEvent()
        => new(WorkspaceId, EventType, GetMetadataJson(), BaseVersion + 1, IdempotencyKey);
    public CreateLearningItemProjectionCommand GetItemStatusProjectionCommand()
    {
        var projectionDto = new CreateLearningItemProjectionDto(Id, null, Status?.FromStatusStringOrDefault(), Type);
        return CreateLearningItemProjectionCommand.Create(WorkspaceId, new List<CreateLearningItemProjectionDto> { projectionDto });
    }
}