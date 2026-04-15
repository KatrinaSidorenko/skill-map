using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.Extensions.Hosting;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class WorkspaceEventsProcessor(IWorkspaceActionStream actionStream, IWorkspaceNotifier notifier) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
