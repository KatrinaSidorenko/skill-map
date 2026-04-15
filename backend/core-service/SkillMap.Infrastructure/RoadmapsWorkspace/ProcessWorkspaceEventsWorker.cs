using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;

/// <summary>
/// Background worker that processes pending workspace events across multiple thread-pool threads.
/// Non-connection events are immediately marked Applied.
/// CreateConnection events are validated for cycles before being accepted or rejected.
/// </summary>
internal sealed class ProcessWorkspaceEventsWorker : BackgroundService
{
    private static readonly TimeSpan _pollingInterval = TimeSpan.FromMilliseconds(2000);
    private const int _workerThreadCount = 4;

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProcessWorkspaceEventsWorker> _logger;
    private readonly Guid _workerId = Guid.NewGuid();

    public ProcessWorkspaceEventsWorker(
        IServiceProvider serviceProvider,
      ILogger<ProcessWorkspaceEventsWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
       "Starting {WorkerName} id={WorkerId} with {ThreadCount} threads",
        nameof(ProcessWorkspaceEventsWorker), _workerId, _workerThreadCount);

        var threads = Enumerable.Range(0, _workerThreadCount).Select(i => RunProcessingLoopAsync(i, stoppingToken));

        await Task.WhenAll(threads);
    }

    private async Task RunProcessingLoopAsync(int threadIndex, CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_pollingInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessNextEventAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                     "{WorkerName} id={WorkerId} thread={ThreadIndex} encountered an error",
                     nameof(ProcessWorkspaceEventsWorker), _workerId, threadIndex);
            }
        }
    }

    private async Task ProcessNextEventAsync(CancellationToken ct)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var eventRepository = scope.ServiceProvider.GetRequiredService<IRoadmapWorkspaceEventRepository>();
        var snapshotRepository = scope.ServiceProvider.GetRequiredService<IRoadmapWorkspaceSnapshotRepository>();

        // Atomically claim one pending event (SKIP LOCKED — safe across threads/processes)
        var pendingEvent = await eventRepository.DequeueNextPendingEventAsync(ct);
        if (pendingEvent is null)
            return;

        _logger.LogInformation(
                    "{WorkerName} id={WorkerId} picked up event id={EventId} type={EventType} workspaceId={WorkspaceId}",
                    nameof(ProcessWorkspaceEventsWorker), _workerId,
                    pendingEvent.Id, pendingEvent.EventType, pendingEvent.RoadmapWorkspaceId);

        if (pendingEvent.EventType == WorkspaceEventType.ConnectionCreated)
            await ApplyConnectionEventAsync(pendingEvent, snapshotRepository, eventRepository, ct);
        else
            pendingEvent.SetStatus(WorkspaceEventStatus.Applied);

        await eventRepository.UpdateAsync(pendingEvent, ct);
        await eventRepository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "{WorkerName} id={WorkerId} event id={EventId} → {Status} (reason: {Reason})",
            nameof(ProcessWorkspaceEventsWorker), _workerId,
            pendingEvent.Id, pendingEvent.EventStatus, pendingEvent.RejectionReason ?? "–");
    }

    private async Task ApplyConnectionEventAsync(
        RoadmapWorkspaceEvent pendingEvent,
        IRoadmapWorkspaceSnapshotRepository snapshotRepository,
        IRoadmapWorkspaceEventRepository eventRepository,
        CancellationToken ct)
    {
        var latestSnapshot = await snapshotRepository.GetLatestSnapshot(pendingEvent.RoadmapWorkspaceId, ct);
        if (latestSnapshot is null)
        {
            _logger.LogWarning(
                    "{WorkerName} id={WorkerId} no snapshot found for workspaceId={WorkspaceId}; rejecting event id={EventId}",
           nameof(ProcessWorkspaceEventsWorker), _workerId,
           pendingEvent.RoadmapWorkspaceId, pendingEvent.Id);
            pendingEvent.SetRejected(WorkspaceEventRejectionReason.CycleDetected);
            return;
        }

        var bridgeEvents = await eventRepository.GetAppliedEventsBetweenAsync(
            pendingEvent.RoadmapWorkspaceId,
            fromVersionExclusive: latestSnapshot.Version,
            toVersionExclusive: pendingEvent.Version,
            ct);

        var currentSnapshot = await latestSnapshot.GetRoadmapSnapshot(ct);
        if (bridgeEvents.Count > 0)
            currentSnapshot = await currentSnapshot.ApplyEventsToSnapshot(bridgeEvents, ct);

        var candidateSnapshot = await currentSnapshot.ApplyEventsToSnapshot([pendingEvent], ct);

        if (TopologicalSort.HasCycle(candidateSnapshot))
        {
            _logger.LogWarning(
                    "{WorkerName} id={WorkerId} cycle detected — rejecting connection event id={EventId} workspaceId={WorkspaceId}",
            nameof(ProcessWorkspaceEventsWorker), _workerId,
                    pendingEvent.Id, pendingEvent.RoadmapWorkspaceId);
            pendingEvent.SetRejected(WorkspaceEventRejectionReason.CycleDetected);
            return;
        }

        pendingEvent.SetStatus(WorkspaceEventStatus.Applied);
    }
}