using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapAssessments.Features.ApplyRoadmapStateSuggestions;

public record ApplyRoadmapStateSuggestionsCommand(
    long AttemptId,
    List<SuggestionItemCommand> Items) : ICommand
{
    public WorkspaceEventType EventType => WorkspaceEventType.LearningItemUpdated;
}

public record SuggestionItemCommand(string Id, string Type, string Status)
{
    public string IdempotencyKey => $"suggestion-{Id}-{Status}";

    public RoadmapWorkspaceEvent ToWorkspaceEvent(long workspaceId, int version) =>
        new(workspaceId,
            WorkspaceEventType.LearningItemUpdated,
            new LearningItemUpdatedEvent(Id, status: Status).JsonSerializeOrDefault(),
            version,
            IdempotencyKey);
    public CreateLearningItemProjectionDto ToProjectionDto() => new(Id, true, Status.FromStatusStringOrDefault());
}
