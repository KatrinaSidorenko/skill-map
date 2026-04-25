namespace LearningPlatform.Workspace.WebSockets.Contracts;
public class RoadmapWorkspaceActionProducerOptions
{
    public const string SectionName = "RoadmapWorkspaceActionProducerOptions";
    public string BootstrapServers { get; set; } = string.Empty;
    public string TopicName { get; set; }
}
