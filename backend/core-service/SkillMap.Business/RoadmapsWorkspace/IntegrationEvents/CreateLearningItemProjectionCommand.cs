using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;

public record CreateLearningItemProjectionDto(string LearningItemId, bool? IsAvailable, LearningStatus? LearningStatus)
{
    public RoadmapLearningProjection ToRoadmapLearningItemProjection(long workspaceId)
        => new(workspaceId, LearningItemId, IsAvailable ?? true, (LearningStatus ?? Core.Constants.LearningStatus.NotStarted).ToStatusString());
    public static CreateLearningItemProjectionDto ToProjectionDto(LearningItemSnapshot learningItemSnapshot)
        => new(learningItemSnapshot.Id, true, learningItemSnapshot.Status);
}
public record CreateLearningItemProjectionCommand(Guid Id, long WorkspaceId, List<CreateLearningItemProjectionDto> ProjectionDtos, DateTimeOffset OccurredDateTime) : IIntegrationEvent
{
    public static CreateLearningItemProjectionCommand Create(long workspaceId, List<CreateLearningItemProjectionDto> projectionDtos)
        => new(Guid.NewGuid(), workspaceId, projectionDtos, DateTimeOffset.UtcNow);
}