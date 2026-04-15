
using LearningPlatform.Workspace.WebSockets;
using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.AspNetCore.SignalR;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class WorkspaceNotifier(IHubContext<WorkspaceHub, IWorkspaceClient> hubContext) : IWorkspaceNotifier
{
    public async Task NotifyActionConfirmed(string workspaceId, int actualVersion, string actionKey, CancellationToken ct)
    {
        await hubContext.Clients.Group(workspaceId).OnActionConfirmed(workspaceId, actualVersion, actionKey, ct);
    }

    public async Task NotifyActionRejected(string workspaceId, int actualVersion, string actionKey, CancellationToken ct)
    {
        await hubContext.Clients.Group(workspaceId).OnActionRejected(workspaceId, actualVersion, actionKey, ct);
    }
}
