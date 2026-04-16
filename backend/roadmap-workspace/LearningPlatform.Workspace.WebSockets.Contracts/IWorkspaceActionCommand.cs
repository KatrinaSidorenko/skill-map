namespace LearningPlatform.Workspace.WebSockets.Contracts;

public interface IWorkspaceActionCommand 
{
    string IdempotencyKey { get; }
}
