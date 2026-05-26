namespace LearningPlatform.Core.IntegrationTests.Engine.MessageBroker;
public record KafkaTopicDescriptor(string Name, int Partitions, short ReplicationFactor);