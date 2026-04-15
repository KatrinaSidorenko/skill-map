using LearningPlatform.Workspace.WebSockets.Contracts;

namespace LearningPlatform.Workspace.WebSockets.Actions;
public abstract class WorkspaceActionRequest
{
    protected abstract IWorkspaceActionCommand ToCommand();
    public abstract WorkspaceAction ToWorkspaceAction(string workspaceId);
}
