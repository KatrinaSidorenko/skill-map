using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;

public record CreateLearningItemConnectionCommand(long WorkspaceId, string Id, string Source, string Target, int ClientWorkspaceVersion) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.CreateConnection;
    public object GetMetadata() => new LearningItemConnectionCreatedEvent(Id, Source, Target);
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
}
