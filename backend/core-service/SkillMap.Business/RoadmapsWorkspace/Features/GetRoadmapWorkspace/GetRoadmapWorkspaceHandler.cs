using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;

[UsedImplicitly]
internal sealed class GetRoadmapWorkspaceHandler(IRoadmapWorkspaceEditor workspaceEditor) : IRequestHandler<GetRoadmapWorkspaceQuery, RoadmapWorkspaceDto>
{
    public async Task<RoadmapWorkspaceDto> Handle(GetRoadmapWorkspaceQuery request, CancellationToken cancellationToken)
    {
        var targetSnapshotContent = await workspaceEditor.GetActualRoadmapSnapshot(request.WorkspaceId, cancellationToken);
        return RoadmapWorkspaceDto.Create(request.WorkspaceId, targetSnapshotContent, targetSnapshotContent.Version);
    }
}