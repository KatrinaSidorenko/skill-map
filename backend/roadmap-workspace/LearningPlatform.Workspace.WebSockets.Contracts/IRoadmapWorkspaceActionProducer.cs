namespace LearningPlatform.Workspace.WebSockets.Contracts;
public interface IRoadmapWorkspaceActionProducer
{
    Task PublishAsync(WorkspaceAction action, CancellationToken ct = default);
}
