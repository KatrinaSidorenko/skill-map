using Testcontainers.Kafka;

namespace LearningPlatform.Core.IntegrationTests.Engine.MessageBroker;

public sealed class KafkaContainer : IAsyncLifetime
{
    private readonly Testcontainers.Kafka.KafkaContainer _container = new KafkaBuilder().Build();

  public string BootstrapServers => _container.GetBootstrapAddress();

    public async Task InitializeAsync() => await _container.StartAsync();

    public async Task DisposeAsync() => await _container.DisposeAsync();
}
