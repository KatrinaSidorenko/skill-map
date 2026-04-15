using LearningPlatform.Workspace.WebSockets.Contracts;

namespace LearningPlatform.Workspace.WebSockets.Actions;
public abstract class WorkspaceActionRequest
{
    public abstract WorkspaceAction ToWorkspaceAction(string workspaceId);
}
