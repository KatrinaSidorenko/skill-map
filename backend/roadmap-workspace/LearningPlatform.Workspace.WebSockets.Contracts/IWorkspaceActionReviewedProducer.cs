namespace LearningPlatform.Workspace.WebSockets.Contracts;

public interface IWorkspaceActionReviewedProducer
{
    Task PublishAsync(WorkspaceActionReviewedEvent reviewedEvent, CancellationToken ct = default);
}
