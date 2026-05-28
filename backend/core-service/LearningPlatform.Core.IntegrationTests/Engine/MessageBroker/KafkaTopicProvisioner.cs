using Confluent.Kafka;
using Confluent.Kafka.Admin;

using LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace;

namespace LearningPlatform.Core.IntegrationTests.Engine.MessageBroker;

public interface IKafkaTopicProvisioner
{
    Task EnsureTopicsExistAsync(string bootstrapServers, List<KafkaTopicDescriptor> topics, CancellationToken ct = default);
}

internal sealed class KafkaTopicProvisioner : IKafkaTopicProvisioner
{
    public async Task EnsureTopicsExistAsync(string bootstrapServers, List<KafkaTopicDescriptor> topics,  CancellationToken ct = default)
    {
        var config = new AdminClientConfig { BootstrapServers = bootstrapServers };

        using var adminClient = new AdminClientBuilder(config).Build();

        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
        var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();

        var topicsToCreate = topics
            .Where(topic => !existingTopics.Contains(topic.Name))
            .Select(topic => new TopicSpecification
            {
                Name = topic.Name,
                NumPartitions = topic.Partitions,
                ReplicationFactor = topic.ReplicationFactor
            })
            .ToList();

        if (topicsToCreate.Count == 0)
            return;

        try
        {
            await adminClient.CreateTopicsAsync(topicsToCreate);
        }
        catch (CreateTopicsException ex)
        {
            foreach (var result in ex.Results.Where(r => r.Error.Code != ErrorCode.TopicAlreadyExists))
            {
                throw new InvalidOperationException($"Failed to create Kafka topic '{result.Topic}': {result.Error.Reason}");
            }
        }
    }
}
