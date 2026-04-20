using LearningPlatform.Workspace.WebSockets.Actions;
using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Workspace.WebSockets;

public class WorkspaceHub : Hub<IWorkspaceClient>
{
    private readonly IWorkspaceActionStream _actionStream;
    private readonly ILogger<WorkspaceHub> _logger;

    public WorkspaceHub(IWorkspaceActionStream actionStream, ILogger<WorkspaceHub> logger)
    {
        _actionStream = actionStream;
        _logger = logger;
    }

    private string GetWorkspaceGroupName(string workspaceId) => workspaceId;

    public async Task Join(string workspaceId)
    {
        if (string.IsNullOrWhiteSpace(workspaceId)) throw new HubException("Workspace ID is required.");

        await Groups.AddToGroupAsync(Context.ConnectionId, GetWorkspaceGroupName(workspaceId));
        _logger.LogInformation("User {UserId} joined workspace {WorkspaceId}", Context.UserIdentifier, workspaceId);
    }

    public async Task Leave(string workspaceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetWorkspaceGroupName(workspaceId));
        _logger.LogInformation("User {UserId} left workspace {WorkspaceId}", Context.UserIdentifier, workspaceId);
    }

    public async Task AddLearningItem(string workspaceId, AddLearningItemAction action)
        => await EnqueueAction(workspaceId, action);

    public async Task UpdateLearningItem(string workspaceId, UpdateLearningItemAction action)
        => await EnqueueAction(workspaceId, action);

    public async Task DeleteLearningItem(string workspaceId, DeleteLearningItemAction action)
        => await EnqueueAction(workspaceId, action);

    public async Task CreateConnection(string workspaceId, CreateLearningItemConnectionAction action)
        => await EnqueueAction(workspaceId, action);

    public async Task DeleteConnection(string workspaceId, DeleteLearningItemConnectionAction action)
        => await EnqueueAction(workspaceId, action);


    private async Task EnqueueAction(string workspaceId, WorkspaceActionRequest payload, CancellationToken ct = default)
    {
        try
        {
            var action = payload.ToWorkspaceAction(workspaceId);
            await _actionStream.EnqueueAction(action, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue action for workspace {WorkspaceId}", workspaceId);
            throw new HubException("The server is temporarily unable to queue your change. Please try again.");
        }
    }
}