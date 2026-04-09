using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;

public record CreateLearningItemStatusProjectionDto(string LearningItemId, bool? IsAvailable, LearningStatus? LearningStatus)
{
    public RoadmapLearningItemStatus ToRoadmapLearningItemStatus(long workspaceId)
        => new(workspaceId, LearningItemId, IsAvailable ?? true, (LearningStatus ?? Core.Constants.LearningStatus.NotStarted).ToStatusString());
    public static CreateLearningItemStatusProjectionDto ToProjectionDto(LearningItemSnapshot learningItemSnapshot)
        => new(learningItemSnapshot.Id, true, learningItemSnapshot.Status);
}
public record CreateLearningItemStatusProjectionCommand(Guid Id, long WorkspaceId, List<CreateLearningItemStatusProjectionDto> ProjectionDtos, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static CreateLearningItemStatusProjectionCommand Create(long workspaceId, List<CreateLearningItemStatusProjectionDto> projectionDtos)
        => new(Guid.NewGuid(), workspaceId, projectionDtos, DateTimeOffset.UtcNow);
}