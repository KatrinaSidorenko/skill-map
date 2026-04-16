namespace LearningPlatform.Workspace.WebSockets.Contracts;
public record WorkspaceActionProcessResult(string IdempotencyKey, WorkspaceActionStatus Status, string? Message = null);
