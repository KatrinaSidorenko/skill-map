namespace SkillMap.Infrastructure.RoadmapsWorkspace;
public class RoadmapWorkspaceActionConsumerOptions
{
    public const string SectionName = "RoadmapWorkspaceActionConsumerOptions";
    public string BootstrapServers { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;
    public bool AutoOffsetResetEarliest { get; set; } = true;
}
