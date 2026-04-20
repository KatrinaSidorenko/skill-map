namespace LearningPlatform.Workspace.WebSockets.Contracts.Commands;

public record DeleteLearningItemActionCommand(
    string Id,
    List<string> IncidentConnectionIds,
    int ClientWorkspaceVersion,
    string IdempotencyKey) : IWorkspaceActionCommand;
