
using JetBrains.Annotations;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;

[UsedImplicitly]
internal sealed class BuildAuthorWorkspaceSnapshotHandler(
    IRepository<RoadmapWorkspaceEvent> repository, 
    ILogger<BuildAuthorWorkspaceSnapshotHandler> logger) : IRequestHandler<BuildAuthorWorkspaceSnapshotCommand, long>
{
    public async Task<long> Handle(BuildAuthorWorkspaceSnapshotCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
