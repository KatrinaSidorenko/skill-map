using LearningPlatform.Core.IntegrationTests.Engine.Configuration;

using SkillMap.Infrastructure.RoadmapsWorkspace;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspaceSnapshot;

internal sealed class WorkspaceActionsConsumerKafkaConfiguration : IOptionsConfiguration
{
    public const string TopicName = "roadmap-workspace-actions";
    public const string ConsumerGroup = "skillmap-workspace-consumers-test";

    private readonly string _bootstrapServers;

    public WorkspaceActionsConsumerKafkaConfiguration(string bootstrapServers)
        => _bootstrapServers = bootstrapServers;

    public Dictionary<string, string?> Get() => new()
    {
        [$"{RoadmapWorkspaceActionConsumerOptions.SectionName}:{nameof(RoadmapWorkspaceActionConsumerOptions.BootstrapServers)}"] = _bootstrapServers,
        [$"{RoadmapWorkspaceActionConsumerOptions.SectionName}:{nameof(RoadmapWorkspaceActionConsumerOptions.TopicName)}"] = TopicName,
        [$"{RoadmapWorkspaceActionConsumerOptions.SectionName}:{nameof(RoadmapWorkspaceActionConsumerOptions.ConsumerGroup)}"] = ConsumerGroup,
        [$"{RoadmapWorkspaceActionConsumerOptions.SectionName}:{nameof(RoadmapWorkspaceActionConsumerOptions.AutoOffsetResetEarliest)}"] = "true",
    };
}
