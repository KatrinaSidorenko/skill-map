using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;
using SkillMap.Core.Tasks;
using SkillMap.Core.Tasks.Input;
using SkillMap.Shared.Extensions;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal sealed class BuildRoadmapWorkspaceSnapshotWorker : BackgroundService
{
    private static readonly TimeSpan _delay = TimeSpan.FromMilliseconds(7000);

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BuildRoadmapWorkspaceSnapshotWorker> _logger;
    private readonly Guid _workerId = Guid.NewGuid();

    public BuildRoadmapWorkspaceSnapshotWorker(IServiceProvider serviceProvider, ILogger<BuildRoadmapWorkspaceSnapshotWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {WorkerName} with id {WorkerId}", nameof(BuildRoadmapWorkspaceSnapshotWorker), _workerId);

        using var timer = new PeriodicTimer(_delay);
        
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var inboxRepository = scope.ServiceProvider.GetRequiredService<IRepository<InboxTask>>();
            InboxTask? task = null;
            try
            {
                task = await LookupForTask(inboxRepository, stoppingToken);
                if (task == null)
                {
                    _logger.LogInformation("No pending tasks found for {WorkerName} with id {WorkerId}", nameof(BuildRoadmapWorkspaceSnapshotWorker), _workerId);
                    continue;
                }
                _logger.LogInformation("Found task with id {TaskId} for {WorkerName} with id {WorkerId}", task.Id, nameof(BuildRoadmapWorkspaceSnapshotWorker), _workerId);
                task.WorkerId = _workerId;
                task.Status = Core.Tasks.TaskStatus.InProgress;
                await inboxRepository.UpdateAsync(task, stoppingToken);
                await inboxRepository.SaveChangesAsync(stoppingToken);

                var input = task.Input.JsonDeserializeOrDefault<BuildWorkspaceSnapshotInput>();
                var result = (long?)null;
                if (input.IsInAuthorMode)
                {
                    var command = new BuildAuthorWorkspaceSnapshotCommand(input.WorkspaceId, input.RoadmapId);
                    result = await mediator.Send(command, stoppingToken);
                }
                else
                {
                   var command = new BuildBlueprintWorkspaceSnapshotCommand(input.WorkspaceId, input.RoadmapId);
                   result = await mediator.Send(command, stoppingToken);
                }

                task.Status = Core.Tasks.TaskStatus.Completed;
                task.Output = new InboxTaskOutput
                {
                    Result = result?.ToString(),
                    IsSuccess = true
                }.JsonSerializeOrDefault();
                await inboxRepository.UpdateAsync(task, stoppingToken);
                await inboxRepository.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Successfully completed task with id {TaskId} for {WorkerName} with id {WorkerId}", task.Id, nameof(BuildRoadmapWorkspaceSnapshotWorker), _workerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while building roadmap workspace snapshot in worker {WorkerId}", _workerId);
                if (task != null)
                {
                    task.Status = Core.Tasks.TaskStatus.Error;
                    task.Output = new InboxTaskOutput
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    }.JsonSerializeOrDefault();
                    await inboxRepository.UpdateAsync(task, stoppingToken);
                    await inboxRepository.SaveChangesAsync(stoppingToken);
                }
            }
        }
    }

    private async Task<InboxTask?> LookupForTask(IRepository<InboxTask> inboxRepository, CancellationToken cancellationToken)
    {
        return await inboxRepository.GetFirstOrDefaultAsync(
            filter: t => 
                t.TaskType == TaskType.BuildWorkspaceSnapshot &&
                t.Status == Core.Tasks.TaskStatus.Pending &&
                t.WorkerId == null,
            ct: cancellationToken);
    }
}
