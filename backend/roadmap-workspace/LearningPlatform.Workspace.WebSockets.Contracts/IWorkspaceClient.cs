namespace LearningPlatform.Workspace.WebSockets.Contracts;
public interface IWorkspaceClient
{
    Task OnActionConfirmed(string workspaceId, int actualVersion, string actionKey, CancellationToken ct);
    Task OnActionRejected(string workspaceId, int actualVersion, string actionKey, CancellationToken ct);
}
