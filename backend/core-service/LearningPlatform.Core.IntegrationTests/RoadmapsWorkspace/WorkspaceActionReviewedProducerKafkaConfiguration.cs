using LearningPlatform.Core.IntegrationTests.Engine.Configuration;
using LearningPlatform.Workspace.WebSockets.Contracts;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace;

internal sealed class WorkspaceActionReviewedProducerKafkaConfiguration : IOptionsConfiguration
{
    public const string TopicName = KafkaTopics.WorkspaceActionReviewed;

    private readonly string _bootstrapServers;

    public WorkspaceActionReviewedProducerKafkaConfiguration(string bootstrapServers)
        => _bootstrapServers = bootstrapServers;

    public Dictionary<string, string?> Get() => new()
    {
        [$"{WorkspaceActionReviewedProducerOptions.SectionName}:{nameof(WorkspaceActionReviewedProducerOptions.BootstrapServers)}"] = _bootstrapServers,
        [$"{WorkspaceActionReviewedProducerOptions.SectionName}:{nameof(WorkspaceActionReviewedProducerOptions.TopicName)}"] = TopicName,
    };
}
