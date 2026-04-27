using System.Text.Json;

using Confluent.Kafka;

using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Workspace.WebSockets;

public class RoadmapWorkspaceActionProducer : IRoadmapWorkspaceActionProducer, IDisposable
{
    private readonly RoadmapWorkspaceActionProducerOptions _options;
    private readonly ILogger<RoadmapWorkspaceActionProducer> _logger;
    private readonly IProducer<string, string> _producer;

    public RoadmapWorkspaceActionProducer(
        IOptions<RoadmapWorkspaceActionProducerOptions> options,
        ILogger<RoadmapWorkspaceActionProducer> logger)
    {
        _options = options.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(WorkspaceAction action, CancellationToken ct = default)
    {
        var key = action.WorkspaceId.ToString();
        var value = JsonSerializer.Serialize(action, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new WorkspaceActionJsonConverter() }
        });

        var message = new Message<string, string>
        {
            Key = key,
            Value = value
        };

        try
        {
            var result = await _producer.ProduceAsync(_options.TopicName, message, ct);
            _logger.LogInformation(
                "Published workspace action {ActionType} for workspace {WorkspaceId} to partition {Partition} at offset {Offset}",
                action.ActionType, action.WorkspaceId, result.Partition.Value, result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex,
                "Failed to publish workspace action {ActionType} for workspace {WorkspaceId}",
                action.ActionType, action.WorkspaceId);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
