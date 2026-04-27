namespace LearningPlatform.Workspace.WebSockets.Contracts;

public class WorkspaceActionReviewedProducerOptions
{
    public const string SectionName = "WorkspaceActionReviewedProducerOptions";

    public string BootstrapServers { get; set; } = string.Empty;
    public string TopicName { get; set; } = "workspace-action-reviewed";
}
