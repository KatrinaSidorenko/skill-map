namespace LearningPlatform.Workspace.WebSockets.Contracts;

public record WorkspaceActionReviewedEvent(
    string WorkspaceId,
    int ActualVersion,
    string ActionKey,
    WorkspaceActionReviewedStatus Status);
