namespace LearningPlatform.Workspace.WebSockets.Contracts;
public record WorkspaceAction(string workspaceId, WorkspaceActionType actionType, IWorkspaceActionCommand payload);
