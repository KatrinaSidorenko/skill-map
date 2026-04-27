using System.Text.Json;

using Confluent.Kafka;

using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SkillMap.Business.RoadmapsWorkspace;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;

internal class RoadmapWorkspaceActionConsumer : BackgroundService, IRoadmapWorkspaceActionConsumer
{
    private readonly RoadmapWorkspaceActionConsumerOptions _options;
    private readonly IWorkspaceEventsReviewer _actionReviewer;
    private readonly ILogger<RoadmapWorkspaceActionConsumer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RoadmapWorkspaceActionConsumer(
        IOptions<RoadmapWorkspaceActionConsumerOptions> options,
        IWorkspaceEventsReviewer actionStream,
        ILogger<RoadmapWorkspaceActionConsumer> logger)
    {
        _options = options.Value;
        _actionReviewer = actionStream;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new WorkspaceActionJsonConverter() }
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);

    public async Task ConsumeAsync(CancellationToken ct)
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

        _logger.LogInformation("Kafka consumer started. Topic: {Topic}, Group: {Group}", _options.TopicName, _options.ConsumerGroup);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;
                try
                {
                    result = consumer.Consume(ct);

                    var action = JsonSerializer.Deserialize<WorkspaceAction>(result.Message.Value, _jsonOptions);
                    if (action is null)
                    {
                        _logger.LogWarning("Received null WorkspaceAction at partition {Partition} offset {Offset}. Skipping.", result.Partition.Value, result.Offset.Value);
                        consumer.Commit(result);
                        continue;
                    }

                    // add retry logic here
                    await _actionReviewer.ReviewAsync(action, ct);
                    consumer.Commit(result);

                    _logger.LogInformation(
                         "Consumed and enqueued {ActionType} for workspace {WorkspaceId} from partition {Partition} offset {Offset}",
                      action.ActionType, action.WorkspaceId, result.Partition.Value, result.Offset.Value);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex,
                       "Failed to deserialize WorkspaceAction from partition {Partition} offset {Offset}. Committing to skip poison message.",
                       result?.Partition.Value, result?.Offset.Value);

                    if (result is not null)
                        consumer.Commit(result);
                }
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex,
                //    "Unexpected error consuming WorkspaceAction from partition {Partition} offset {Offset}.",
                //        result?.Partition.Value, result?.Offset.Value);
                //}
            }
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer stopped. Topic: {Topic}", _options.TopicName);
        }
    }
}
