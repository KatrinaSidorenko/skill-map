namespace LearningPlatform.Workspace.WebSockets.Contracts.Actions;
public abstract class WorkspaceActionRequest
{
    public abstract WorkspaceAction ToWorkspaceAction(string workspaceId);
}
