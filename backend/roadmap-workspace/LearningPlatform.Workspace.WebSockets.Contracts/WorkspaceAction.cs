namespace LearningPlatform.Workspace.WebSockets.Contracts;
public record WorkspaceAction(long WorkspaceId, WorkspaceActionType ActionType, IWorkspaceActionCommand Payload);
