using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SkillMap.Business;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.AddLearningItem;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItem;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItemConnection;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.UpdateLearningItem;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class WorkspaceEventsProcessor(
    ILogger<WorkspaceEventsProcessor> logger,
    IWorkspaceActionStream actionStream,
    IWorkspaceNotifier notifier,
    IMediator mediator,
    IRoadmapWorkspaceEventRepository eventsRepository) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var action in actionStream.SubscribeToActions(stoppingToken))
        {
            try
            {
                var result = await ProcessActionAsync(action, stoppingToken);
                //await notifier.NotifyWorkspaceChangedAsync(action.WorkspaceId, roadmapId, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing workspace action for workspace {WorkspaceId}", action.WorkspaceId);
            }
        }
    }

    private async Task<WorkspaceActionProcessResult> ProcessActionAsync(WorkspaceAction action, CancellationToken ct)
    {
        try
        {
            // todo: add check IndempotencyKey to prevent processing the same action multiple times in case of retries, maybe store it in cache with expiration
            var lastWorkspaceEventVersion = await eventsRepository.GetLastAvailableEventVersion(action.WorkspaceId, ct);
            var command = ToCommand(action.WorkspaceId, action.Payload, lastWorkspaceEventVersion);
            await mediator.Send(command, ct);
            return new WorkspaceActionProcessResult(action.Payload.IdempotencyKey, WorkspaceActionStatus.Applied);
        }
        catch (Exception ex)
        {
            // todo: add typed exception like cycles detected
            logger.LogError(ex, "Error processing workspace action for workspace {WorkspaceId}", action.WorkspaceId);
            return new WorkspaceActionProcessResult(action.Payload.IdempotencyKey, WorkspaceActionStatus.Rejected);
        }
    }

    private ICommand ToCommand(long workspaceId, IWorkspaceActionCommand command, int baseVersion) => command switch
    {
        AddLearningItemActionCommand c => new AddLearningItemCommand(
            workspaceId, c.Id, c.Title, c.Description, c.Status, c.Type, baseVersion, c.IdempotencyKey),
        UpdateLearningItemActionCommand c => new UpdateLearningItemCommand(
            workspaceId, c.Id, c.Title, c.Description, c.Status, c.Type, baseVersion, c.IdempotencyKey),
        DeleteLearningItemActionCommand c => new DeleteLearningItemCommand(
            workspaceId, c.Id, baseVersion, c.IdempotencyKey),
        CreateLearningItemConnectionActionCommand c => new CreateLearningItemConnectionCommand(
            workspaceId, c.Id, c.Source, c.Target, baseVersion, c.IdempotencyKey),
        DeleteLearningItemConnectionActionCommand c => new DeleteLearningItemConnectionCommand(
            workspaceId, c.Id, baseVersion, c.IdempotencyKey),
        _ => throw new ArgumentOutOfRangeException(nameof(command), $"Unsupported command type: {command.GetType()}")
    };
}
