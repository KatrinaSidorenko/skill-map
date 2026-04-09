
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.Constants;
using SkillMap.Shared.EventBus;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateLearningItemsStatusProjection;
internal class CreateLearningItemsStatusProjectionHandler(IRoadmapLearningItemStatusRepository repository) : IIntegrationEventHandler<CreateLearningItemStatusProjectionCommand>
{
    public async Task Handle(CreateLearningItemStatusProjectionCommand notification, CancellationToken cancellationToken)
    {
        var workspaceProjections = await repository.GetAllAsync(p => p.RoadmapWorkspaceId == notification.WorkspaceId,  ct: cancellationToken);
        var workspaceProjectionsDict = workspaceProjections.ToDictionary(p => p.LearningItemId, p => p);
        foreach (var projectionDto in notification.ProjectionDtos)
        {
            var existingProjection = workspaceProjectionsDict.GetOrDefault(projectionDto.LearningItemId);
            if (existingProjection != null)
            {
                existingProjection.UpdateStatus(projectionDto.LearningStatus.HasValue ? projectionDto.LearningStatus.Value.ToStatusString() : null);
                existingProjection.UpdateAvailability(projectionDto.IsAvailable);
                await repository.UpdateAsync(existingProjection, cancellationToken);
            }
            else
            {
                var newProjection = projectionDto.ToRoadmapLearningItemStatus(notification.WorkspaceId);
                await repository.AddAsync(newProjection, cancellationToken);
            }
        }
        await repository.SaveChangesAsync(cancellationToken);
    }
}
