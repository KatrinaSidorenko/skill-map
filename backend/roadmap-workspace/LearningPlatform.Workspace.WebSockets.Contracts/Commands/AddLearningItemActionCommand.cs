namespace LearningPlatform.Workspace.WebSockets.Contracts.Commands;
public record AddLearningItemActionCommand(
    string Id, 
    string Title, 
    string Description, 
    string Status, 
    string Type, 
    int ClientWorkspaceVersion, 
    string IdempotencyKey) : IWorkspaceActionCommand;