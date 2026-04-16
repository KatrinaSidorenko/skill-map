namespace LearningPlatform.Workspace.WebSockets.Contracts;
public interface IWorkspaceNotifier
{
    Task NotifyActionConfirmed(string workspaceId, int actualVersion, string actionKey, CancellationToken ct);
    Task NotifyActionRejected(string workspaceId, int actualVersion, string actionKey, CancellationToken ct);
}
