namespace LearningPlatform.Workspace.WebSockets.Contracts.Commands;

public record DeleteLearningItemConnectionActionCommand(
    string Id,
    int ClientWorkspaceVersion,
    string IdempotencyKey) : IWorkspaceActionCommand;
