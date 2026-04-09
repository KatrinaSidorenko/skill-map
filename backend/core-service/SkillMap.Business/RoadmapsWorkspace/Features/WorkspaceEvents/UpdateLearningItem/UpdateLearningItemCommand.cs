using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.UpdateLearningItem;

public record UpdateLearningItemCommand(long WorkspaceId, string Id, string? Title, string? Description, string? Status, string? Type, int ClientWorkspaceVersion, string IdempotencyKey) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.UpdateLearningItem;
    public object GetMetadata()
        => new LearningItemUpdatedEvent(Id, Title, Description, Status, Type);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
    public RoadmapWorkspaceEvent ToRoadmapWorkspaceEvent(int version)
        => new(WorkspaceId, EventType, GetMetadataJson(), version, IdempotencyKey);
    public CreateLearningItemStatusProjectionCommand GetItemStatusProjectionCommand()
    {
        var projectionDto = new CreateLearningItemStatusProjectionDto(Id, null, Status?.FromStatusStringOrDefault());
        return CreateLearningItemStatusProjectionCommand.Create(WorkspaceId, new List<CreateLearningItemStatusProjectionDto> { projectionDto });
    }
}