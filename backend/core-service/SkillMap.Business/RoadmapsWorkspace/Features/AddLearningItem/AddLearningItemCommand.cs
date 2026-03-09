using SkillMap.Core.Constants;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.AddLearningItem;
public record AddLearningItemCommand(long UserRoadmapId, string Title, string Description, string Status, int ClientWorkspaceVersion) : ICommand
{
    public EventType EventType => EventType.Create;
    public object GetMetadata()
    {
        return new
        {
            Title,
            Description,
            Status,
            ClientWorkspaceVersion,
        };
    }
}
