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

        if (request.Title is not null)
            workspace.UpdateTitle(request.Title);

        if (request.Description is not null)
            workspace.UpdateDescription(request.Description);

        if (request.ImageUrl is not null)
            workspace.UpdateImageUrl(request.ImageUrl);

        await repository.UpdateAsync(workspace, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
