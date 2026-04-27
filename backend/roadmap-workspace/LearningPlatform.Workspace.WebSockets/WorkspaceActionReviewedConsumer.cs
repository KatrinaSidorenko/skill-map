using System.Text.Json;

using Confluent.Kafka;

using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Workspace.WebSockets;
internal class WorkspaceActionReviewedConsumer : BackgroundService
{
    private readonly WorkspaceActionReviewedConsumerOptions _options;
    private readonly IHubContext<WorkspaceHub, IWorkspaceClient> _hubContext;
    private readonly ILogger<WorkspaceActionReviewedConsumer> _logger;

    public WorkspaceActionReviewedConsumer(
        IOptions<WorkspaceActionReviewedConsumerOptions> options,
        IHubContext<WorkspaceHub, IWorkspaceClient> hubContext,
        ILogger<WorkspaceActionReviewedConsumer> logger)
    {
        _options = options.Value;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);

    private async Task ConsumeAsync(CancellationToken ct)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.ConsumerGroup,
            AutoOffsetReset = _options.AutoOffsetResetEarliest
                ? AutoOffsetReset.Earliest
                : AutoOffsetReset.Latest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_options.TopicName);

        _logger.LogInformation(
          "WorkspaceActionReviewedConsumer started. Topic: {Topic}, Group: {Group}",
               _options.TopicName, _options.ConsumerGroup);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;
                try
                {
                    result = consumer.Consume(ct);

                    var reviewedEvent = JsonSerializer.Deserialize<WorkspaceActionReviewedEvent>(
                          result.Message.Value,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (reviewedEvent is null)
                    {
                        _logger.LogWarning(
                             "Null WorkspaceActionReviewedEvent at partition {Partition} offset {Offset}. Skipping.",
                        result.Partition.Value, result.Offset.Value);
                        consumer.Commit(result);
                        continue;
                    }

                    // todo: add retry logic
                    await DispatchToHubAsync(reviewedEvent, ct);
                    consumer.Commit(result);

                    _logger.LogDebug(
                        "Dispatched reviewed event {Status} for workspace {WorkspaceId} action {ActionKey}",
                            reviewedEvent.Status, reviewedEvent.WorkspaceId, reviewedEvent.ActionKey);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to deserialize WorkspaceActionReviewedEvent at partition {Partition} offset {Offset}. Skipping poison message.",
                        result?.Partition.Value, result?.Offset.Value);

                    if (result is not null)
                        consumer.Commit(result);
                }
                // todo: think about better error handling and in error cases
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Unexpected error consuming WorkspaceActionReviewedEvent at partition {Partition} offset {Offset}.",
                        result?.Partition.Value, result?.Offset.Value);
                }
            }
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("WorkspaceActionReviewedConsumer stopped. Topic: {Topic}", _options.TopicName);
        }
    }

    private Task DispatchToHubAsync(WorkspaceActionReviewedEvent reviewedEvent, CancellationToken ct)
    => reviewedEvent.Status switch
    {
        WorkspaceActionReviewedStatus.Confirmed =>
        _hubContext.Clients.Group(reviewedEvent.WorkspaceId)
              .OnActionConfirmed(reviewedEvent.WorkspaceId, reviewedEvent.ActualVersion, reviewedEvent.ActionKey, ct),

        WorkspaceActionReviewedStatus.Rejected =>
       _hubContext.Clients.Group(reviewedEvent.WorkspaceId)
          .OnActionRejected(reviewedEvent.WorkspaceId, reviewedEvent.ActualVersion, reviewedEvent.ActionKey, ct),

        _ => Task.CompletedTask
    };
}
