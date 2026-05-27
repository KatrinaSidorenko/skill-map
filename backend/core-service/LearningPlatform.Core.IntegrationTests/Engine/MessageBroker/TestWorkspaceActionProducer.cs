using System.Text.Json;

using Confluent.Kafka;

using LearningPlatform.Workspace.WebSockets.Contracts;

namespace LearningPlatform.Core.IntegrationTests.Engine.MessageBroker;

internal sealed class TestWorkspaceActionProducer : IDisposable
{
    private readonly string _topicName;
    private readonly IProducer<string, string> _producer;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new WorkspaceActionJsonConverter() }
    };

    public TestWorkspaceActionProducer(string bootstrapServers, string topicName)
    {
        _topicName = topicName;
        _producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
        }).Build();
    }

    public async Task PublishAsync(WorkspaceAction action, CancellationToken ct = default)
    {
        var message = new Message<string, string>
        {
            Key = action.WorkspaceId.ToString(),
            Value = JsonSerializer.Serialize(action, _jsonOptions)
        };

        await _producer.ProduceAsync(_topicName, message, ct);
    }

    public async Task PublishManyAsync(IEnumerable<WorkspaceAction> actions, CancellationToken ct = default)
    {
        foreach (var action in actions)
            await PublishAsync(action, ct);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
