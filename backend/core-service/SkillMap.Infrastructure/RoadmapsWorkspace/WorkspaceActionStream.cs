
using System.Runtime.CompilerServices;
using System.Threading.Channels;

using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.Extensions.Logging;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class WorkspaceActionStream : IWorkspaceActionStream
{
    private readonly ILogger<WorkspaceActionStream> _logger;
    private readonly Channel<WorkspaceAction> _channel = Channel.CreateUnbounded<WorkspaceAction>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });
    public WorkspaceActionStream(ILogger<WorkspaceActionStream> logger)
    {
        _logger = logger;
    }
    public async Task EnqueueAction(WorkspaceAction action, CancellationToken ct)
    {
        await _channel.Writer.WriteAsync(action, ct);
    }

    public async IAsyncEnumerable<WorkspaceAction> SubscribeToActions([EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var action = await _channel.Reader.ReadAsync(ct);
            yield return action;
        }
    }
}
