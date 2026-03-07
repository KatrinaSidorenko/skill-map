using SkillMap.Core.Constants;

namespace SkillMap.Business.PersonalizedRoadmaps.AddLearningItem;
public record AddLearningItemCommand(long UserRoadmapId, string Title, string Description, string Status) : ICommand
{
    public EventType EventType => EventType.Create;
    public object GetMetadata()
    {
        return new
        {
            Title,
            Description,
            Status
        };
    }
}
