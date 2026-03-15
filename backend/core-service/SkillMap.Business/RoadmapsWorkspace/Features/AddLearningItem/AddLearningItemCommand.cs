using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.AddLearningItem;
public record AddLearningItemCommand(long UserRoadmapId, string Id, string Title, string Description, string Status, int ClientWorkspaceVersion) : ICommand
{
    public EventType EventType => EventType.CreateLearningItem;
    public object GetMetadata()
        => new LearningItemCreatedEvent(Id, Title, Description, Status ?? LearningStatus.NotStarted.ToStatusString());
    public string GetMetadataJson() => GetMetadata().JsonSerializeOrDefault();
}
