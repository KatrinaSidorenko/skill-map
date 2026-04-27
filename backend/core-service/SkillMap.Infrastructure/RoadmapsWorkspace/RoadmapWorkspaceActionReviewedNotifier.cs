using System.Text.Json;

using Confluent.Kafka;

using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;

internal class RoadmapWorkspaceActionReviewedNotifier : IRoadmapWorkspaceActionReviewedNotifier, IDisposable
{
    private readonly WorkspaceActionReviewedProducerOptions _options;
    private readonly ILogger<RoadmapWorkspaceActionReviewedNotifier> _logger;
    private readonly IProducer<string, string> _producer;

    public RoadmapWorkspaceActionReviewedNotifier(
        IOptions<WorkspaceActionReviewedProducerOptions> options,
      ILogger<RoadmapWorkspaceActionReviewedNotifier> logger)
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

    public Task NotifyActionConfirmed(string workspaceId, int actualVersion, string actionKey, CancellationToken ct)
     => PublishAsync(new WorkspaceActionReviewedEvent(
      workspaceId, actualVersion, actionKey, WorkspaceActionReviewedStatus.Confirmed), ct);

    public Task NotifyActionRejected(string workspaceId, int actualVersion, string actionKey, CancellationToken ct)
        => PublishAsync(new WorkspaceActionReviewedEvent(
      workspaceId, actualVersion, actionKey, WorkspaceActionReviewedStatus.Rejected), ct);

    private async Task PublishAsync(WorkspaceActionReviewedEvent reviewedEvent, CancellationToken ct)
    {
        var value = JsonSerializer.Serialize(reviewedEvent);
        var message = new Message<string, string>
        {
            Key = reviewedEvent.WorkspaceId,
            Value = value
        };

        try
        {
            var result = await _producer.ProduceAsync(_options.TopicName, message, ct);
            _logger.LogInformation(
                "Published reviewed event {Status} for workspace {WorkspaceId} action {ActionKey} to partition {Partition} at offset {Offset}",
                reviewedEvent.Status, reviewedEvent.WorkspaceId, reviewedEvent.ActionKey,
                result.Partition.Value, result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex,
                "Failed to publish reviewed event {Status} for workspace {WorkspaceId} action {ActionKey}",
                reviewedEvent.Status, reviewedEvent.WorkspaceId, reviewedEvent.ActionKey);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
