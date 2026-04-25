using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
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

public interface IWorkspaceEventsProcessor
{
    Task ProcessAsync(WorkspaceAction action, CancellationToken stoppingToken);
}
internal class WorkspaceEventsProcessor(
    ILogger<WorkspaceEventsProcessor> logger,
    IWorkspaceNotifier notifier,
    IServiceProvider serviceProvider) : IWorkspaceEventsProcessor
{
    public async Task ProcessAsync(WorkspaceAction action, CancellationToken stoppingToken)
    {
        try
        {
            var (result, actualVersion) = await ProcessActionAsync(action, stoppingToken);
            if (result.Status == WorkspaceActionStatus.Applied)
            {
                await notifier.NotifyActionConfirmed(action.WorkspaceId.ToString(), actualVersion, result.IdempotencyKey, stoppingToken);
            }
            else if (result.Status == WorkspaceActionStatus.Rejected)
            {
                await notifier.NotifyActionRejected(action.WorkspaceId.ToString(), actualVersion, result.IdempotencyKey, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing workspace action for workspace {WorkspaceId}", action.WorkspaceId);
        }
    }

    private async Task<(WorkspaceActionProcessResult Result, int ActualVersion)> ProcessActionAsync(WorkspaceAction action, CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var eventsRepository = scope.ServiceProvider.GetRequiredService<IRoadmapWorkspaceEventRepository>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var lastWorkspaceEventVersion = await eventsRepository.GetLastAvailableEventVersion(action.WorkspaceId, ct);
        try
        {
            if (await eventsRepository.IsEventExist(action.WorkspaceId, action.Payload.IdempotencyKey, ct))
            {
                logger.LogInformation("Workspace action with idempotency key {IdempotencyKey} already exists for workspace {WorkspaceId}", action.Payload.IdempotencyKey, action.WorkspaceId);
                return (new WorkspaceActionProcessResult(action.Payload.IdempotencyKey, WorkspaceActionStatus.Applied), lastWorkspaceEventVersion);
            }

            var command = ToCommand(action.WorkspaceId, action.Payload, lastWorkspaceEventVersion);
            await mediator.Send(command, ct);
            return (new WorkspaceActionProcessResult(action.Payload.IdempotencyKey, WorkspaceActionStatus.Applied), lastWorkspaceEventVersion + 1);
        }
        catch (Exception ex)
        {
            // todo: add typed exception like cycles detected
            logger.LogError(ex, "Error processing workspace action for workspace {WorkspaceId}", action.WorkspaceId);
            return (new WorkspaceActionProcessResult(action.Payload.IdempotencyKey, WorkspaceActionStatus.Rejected), lastWorkspaceEventVersion);
        }
    }

    private ICommand ToCommand(long workspaceId, IWorkspaceActionCommand command, int baseVersion) => command switch
    {
        AddLearningItemActionCommand c => new AddLearningItemCommand(
            workspaceId, c.Id, c.Title, c.Description, c.Status, c.Type, baseVersion, c.IdempotencyKey),
        UpdateLearningItemActionCommand c => new UpdateLearningItemCommand(
            workspaceId, c.Id, c.Title, c.Description, c.Status, c.Type, baseVersion, c.IdempotencyKey),
        DeleteLearningItemActionCommand c => new DeleteLearningItemCommand(
            workspaceId, c.Id, c.IncidentConnectionIds, baseVersion, c.IdempotencyKey),
        CreateLearningItemConnectionActionCommand c => new CreateLearningItemConnectionCommand(
            workspaceId, c.Id, c.Source, c.Target, baseVersion, c.IdempotencyKey),
        DeleteLearningItemConnectionActionCommand c => new DeleteLearningItemConnectionCommand(
            workspaceId, c.Id, baseVersion, c.IdempotencyKey),
        _ => throw new ArgumentOutOfRangeException(nameof(command), $"Unsupported command type: {command.GetType()}")
    };
}
