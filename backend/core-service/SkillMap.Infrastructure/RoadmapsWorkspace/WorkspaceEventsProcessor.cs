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
    IRoadmapWorkspaceEventRepository eventsRepository,
    IRoadmapWorkspaceEditor workspaceEditor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var action in actionStream.SubscribeToActions(stoppingToken))
        {
            try
            {
                var result = await ProcessActionAsync(action, stoppingToken);
                await notifier.NotifyWorkspaceChangedAsync(action.WorkspaceId, roadmapId, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing workspace action for workspace {WorkspaceId}", action.WorkspaceId);
            }
        }
    }

    private async Task<ProcessedActionDto> ProcessActionAsync(WorkspaceAction action, CancellationToken ct)
    {
        try
        {
            var result = await mediator.Send(ToWorkspaceEvent(action.Payload), ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing workspace action for workspace {WorkspaceId}", action.WorkspaceId);
            return new ProcessedActionDto(action.Payload.IdempotencyKey, WorkspaceActionStatus.Error);
        }
    }

    private ICommand ToCommand(long workspaceId, IWorkspaceActionCommand command) => command switch
    {
        AddLearningItemActionCommand c => new AddLearningItemCommand(
            workspaceId, c.Id, c.Title, c.Description, c.Status, c.Type, c.ClientWorkspaceVersion, c.IdempotencyKey),
        UpdateLearningItemActionCommand c => new UpdateLearningItemCommand(
            workspaceId, c.Id, c.Title, c.Description, c.Status, c.Type, c.ClientWorkspaceVersion, c.IdempotencyKey),
        DeleteLearningItemActionCommand c => new DeleteLearningItemCommand(
            workspaceId, c.Id, c.ClientWorkspaceVersion, c.IdempotencyKey),
        CreateLearningItemConnectionActionCommand c => new CreateLearningItemConnectionCommand(
            workspaceId, c.Id, c.Source, c.Target, c.ClientWorkspaceVersion, c.IdempotencyKey),
        DeleteLearningItemConnectionActionCommand c => new DeleteLearningItemConnectionCommand(
            workspaceId, c.Id, c.ClientWorkspaceVersion, c.IdempotencyKey),
        _ => throw new ArgumentOutOfRangeException(nameof(command), $"Unsupported command type: {command.GetType()}")
    };
}
