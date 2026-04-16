using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Core.Tasks;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
public class CheckRoadmapWorkspaceIsActiveWorker : BackgroundService
{
    private static readonly TimeSpan _delay = TimeSpan.FromMilliseconds(7000);
    private static readonly TimeSpan _workspaceInactivityThreshold = TimeSpan.FromMinutes(10);
    private readonly ILogger<CheckRoadmapWorkspaceIsActiveWorker> _logger;
    private readonly IRepository<InboxTask> _inboxRepository;

    public CheckRoadmapWorkspaceIsActiveWorker(ILogger<CheckRoadmapWorkspaceIsActiveWorker> logger, IRepository<InboxTask> inboxRepository)
    {
        _logger = logger;
        _inboxRepository = inboxRepository;
    }
    // todo: implement
    // takes last events by workspace id and if the last events was 10 minutes ago create inbox task to create snapshot
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
