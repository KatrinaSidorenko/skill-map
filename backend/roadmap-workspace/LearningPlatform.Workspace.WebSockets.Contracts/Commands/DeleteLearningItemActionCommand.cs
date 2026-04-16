namespace LearningPlatform.Workspace.WebSockets.Contracts.Commands;

public record DeleteLearningItemActionCommand(
    string Id,
    int ClientWorkspaceVersion,
    string IdempotencyKey) : IWorkspaceActionCommand;
