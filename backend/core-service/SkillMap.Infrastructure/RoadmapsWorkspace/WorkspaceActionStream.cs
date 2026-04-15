
using LearningPlatform.Workspace.WebSockets.Contracts;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class WorkspaceActionStream : IWorkspaceActionStream
{
    public async Task EnqueueAction(WorkspaceAction action, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<WorkspaceAction> SubscribeToActions(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
