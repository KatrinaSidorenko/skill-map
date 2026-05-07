using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace.Features.UpdateRoadmapWorkspace;

[UsedImplicitly]
internal sealed class UpdateRoadmapWorkspaceHandler(IRoadmapWorkspaceRepository repository) : IRequestHandler<UpdateRoadmapWorkspaceCommand>
{
    public async Task Handle(UpdateRoadmapWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await repository.GetUserActiveWorkspace(request.WorkspaceId, cancellationToken);

        var current = workspace.Metadata;

        var updated = new RoadmapWorkspaceMetadata(
        request.Title ?? current.Title,
        request.Description ?? current.Description,
        request.ImageUrl ?? current.ImageUrl);

        workspace.SetMetadata(updated);

        await repository.UpdateAsync(workspace, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
