using LearningPlatform.Workspace.WebSockets.Contracts;

namespace LearningPlatform.Workspace.WebSockets.Contracts.Commands;

public record CreateLearningItemConnectionActionCommand(
    string Id,
    string Source,
    string Target,
    int ClientWorkspaceVersion,
    string IdempotencyKey) : IWorkspaceActionCommand;