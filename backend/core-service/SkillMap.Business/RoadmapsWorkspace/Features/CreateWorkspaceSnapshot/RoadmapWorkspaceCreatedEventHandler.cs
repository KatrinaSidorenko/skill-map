
using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.Tasks;
using SkillMap.Core.Tasks.Input;
using SkillMap.Shared.EventBus;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;

[UsedImplicitly]
internal sealed class RoadmapWorkspaceCreatedEventHandler(IRepository<InboxTask> repository, ILogger<RoadmapWorkspaceCreatedEventHandler> logger) : IIntegrationEventHandler<RoadmapWorkspaceCreatedEvent>
{
    public async Task Handle(RoadmapWorkspaceCreatedEvent notification, CancellationToken cancellationToken)
    {
        var task = new InboxTask(new BuildWorkspaceSnapshotInput
        {
            WorkspaceId = notification.WorkspaceId,
            RoadmapId = notification.RoadmapId,
            IsInAuthorMode = notification.IsInAuthorMode,
        }.JsonSerializeOrDefault(), TaskType.BuildWorkspaceSnapshot);

        await repository.AddAsync(task, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Enqueued BuildWorkspaceSnapshot task with id {TaskId} for workspace {WorkspaceId}", task.Id, notification.WorkspaceId);
    }
}