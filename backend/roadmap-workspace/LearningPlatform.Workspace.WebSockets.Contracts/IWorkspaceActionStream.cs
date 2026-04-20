using LearningPlatform.Workspace.WebSockets.Contracts;

namespace LearningPlatform.Workspace.WebSockets.Contracts;

public interface IWorkspaceActionStream
{
    Task EnqueueAction(WorkspaceAction action, CancellationToken ct);
    IAsyncEnumerable<WorkspaceAction> SubscribeToActions(CancellationToken ct);
}
