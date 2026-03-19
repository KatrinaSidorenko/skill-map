using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItem;

public record DeleteLearningItemCommand(long WorkspaceId, string Id, int ClientWorkspaceVersion) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.DeleteLearningItem;
    public object GetMetadata() => new LearningItemDeletedEvent(Id);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
}
