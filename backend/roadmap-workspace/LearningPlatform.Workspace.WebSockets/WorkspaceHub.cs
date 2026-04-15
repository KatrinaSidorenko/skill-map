using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Actions;

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

    private string GetWorkspaceGroupName(string workspaceId) => $"workspace-{workspaceId}";

    public async Task Join(string workspaceId)
    {
        if (string.IsNullOrWhiteSpace(workspaceId)) throw new HubException("Workspace ID is required.");

        await Groups.AddToGroupAsync(Context.ConnectionId, GetWorkspaceGroupName(workspaceId));
        _logger.LogInformation("User {UserId} joined workspace {WorkspaceId}", Context.UserIdentifier, workspaceId);
    }

    public async Task Leave(string workspaceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetWorkspaceGroupName(workspaceId));
    }

    public async Task AddLearningItem(string workspaceId, AddLearningItemAction action, CancellationToken ct)
        => await EnqueueAction(workspaceId, action, ct);

    public async Task UpdateLearningItem(string workspaceId, UpdateLearningItemAction action, CancellationToken ct)
        => await EnqueueAction(workspaceId, action, ct);

    public async Task DeleteLearningItem(string workspaceId, DeleteLearningItemAction action, CancellationToken ct)
        => await EnqueueAction(workspaceId, action, ct);

    public async Task CreateConnection(string workspaceId, CreateLearningItemConnectionAction action, CancellationToken ct)
        => await EnqueueAction(workspaceId, action, ct);

    public async Task DeleteConnection(string workspaceId, DeleteLearningItemConnectionAction action, CancellationToken ct)
        => await EnqueueAction(workspaceId, action, ct);


    private async Task EnqueueAction(string workspaceId, WorkspaceActionRequest payload, CancellationToken ct)
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