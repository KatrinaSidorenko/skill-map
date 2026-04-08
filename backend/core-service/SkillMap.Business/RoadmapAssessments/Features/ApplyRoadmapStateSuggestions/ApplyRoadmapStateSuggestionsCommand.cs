using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapAssessments.Features.ApplyRoadmapStateSuggestions;

public record ApplyRoadmapStateSuggestionsCommand(
    long AttemptId,
    List<SuggestionItemCommand> Items) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.UpdateLearningItem;
}

public record SuggestionItemCommand(string Id, string Type, string Status)
{
    public string IdempotencyKey => $"suggestion-{Id}-{Status}";

    public RoadmapWorkspaceEvent ToWorkspaceEvent(long workspaceId, int version) =>
        new(workspaceId,
            WorkspaceEventType.UpdateLearningItem,
            new LearningItemUpdatedEvent(Id, status: Status).JsonSerializeOrDefault(),
            version,
            IdempotencyKey);
}
