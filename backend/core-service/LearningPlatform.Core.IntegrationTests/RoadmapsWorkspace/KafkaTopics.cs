using LearningPlatform.Core.IntegrationTests.Engine.MessageBroker;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace;

public static class KafkaTopics
{
    public const string WorkspaceActions = "roadmap-workspace-actions";
    public const string WorkspaceActionReviewed = "workspace-action-reviewed";

    public static IReadOnlyList<KafkaTopicDescriptor> GetAllTopics() =>
    [
        new KafkaTopicDescriptor(WorkspaceActions, Partitions: 1, ReplicationFactor: 1),
        new KafkaTopicDescriptor(WorkspaceActionReviewed, Partitions: 1, ReplicationFactor: 1)
    ];
}