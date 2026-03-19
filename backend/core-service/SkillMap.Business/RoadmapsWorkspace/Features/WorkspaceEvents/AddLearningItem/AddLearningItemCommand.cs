using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.AddLearningItem;
public record AddLearningItemCommand(long WorkspaceId, string Id, string Title, string Description, string Status, int ClientWorkspaceVersion) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.CreateLearningItem;
    public object GetMetadata()
        => new LearningItemCreatedEvent(Id, Title, Description, Status ?? LearningStatus.NotStarted.ToStatusString());
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
}
