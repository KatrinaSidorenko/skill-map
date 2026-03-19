using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItemConnection;

public record DeleteLearningItemConnectionCommand(long WorkspaceId, string Id, int ClientWorkspaceVersion) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.DeleteConnection;
    public object GetMetadata() => new LearningItemConnectionDeletedEvent(Id);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
}
