namespace LearningPlatform.Workspace.WebSockets.Contracts;

public class WorkspaceActionReviewedConsumerOptions
{
    public const string SectionName = "WorkspaceActionReviewedConsumerOptions";

    public string BootstrapServers { get; set; } = string.Empty;
    public string TopicName { get; set; } = "workspace-action-reviewed";
    public string ConsumerGroup { get; set; } = "workspace-reviewed-consumers";
    public bool AutoOffsetResetEarliest { get; set; } = true;
}
