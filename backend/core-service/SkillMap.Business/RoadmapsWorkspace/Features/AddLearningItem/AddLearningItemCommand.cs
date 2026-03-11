using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.Events;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.AddLearningItem;
public record AddLearningItemCommand(long UserRoadmapId, string Title, string Description, string Status, int ClientWorkspaceVersion) : ICommand
{
    public EventType EventType => EventType.CreateLearningItem;
    public object GetMetadata()
        => new LearningItemCreatedEvent(Title, Description, Status ?? LearningStatus.NotStarted.ToStatusString());
}
