using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.UpdateLearningItem;

public record UpdateLearningItemCommand(long WorkspaceId, string Id, string? Title, string? Description, string? Status, int ClientWorkspaceVersion) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.UpdateLearningItem;
    public object GetMetadata()
        => new LearningItemUpdatedEvent(Id, Title, Description, Status);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
}
