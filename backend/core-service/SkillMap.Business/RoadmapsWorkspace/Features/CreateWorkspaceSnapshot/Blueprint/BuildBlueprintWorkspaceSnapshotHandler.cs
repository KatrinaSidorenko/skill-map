
using JetBrains.Annotations;

using MediatR;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot.Blueprint;
[UsedImplicitly]
internal sealed class BuildBlueprintWorkspaceSnapshotHandler : IRequestHandler<BuildBlueprintWorkspaceSnapshotCommand, long>
{
    public Task<long> Handle(BuildBlueprintWorkspaceSnapshotCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
