using JetBrains.Annotations;

using MediatR;

namespace SkillMap.Business.RoadmapsWorkspace.Features.UpdateRoadmapWorkspace;

[UsedImplicitly]
internal sealed class UpdateRoadmapWorkspaceHandler(IRoadmapWorkspaceRepository repository, IRoadmapWorkspaceImageService roadmapWorkspaceImageService) : IRequestHandler<UpdateRoadmapWorkspaceCommand>
{
    public async Task Handle(UpdateRoadmapWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await repository.GetUserActiveWorkspace(request.WorkspaceId, cancellationToken);

        if (request.Title is not null)
            workspace.UpdateTitle(request.Title);

        if (request.Description is not null)
            workspace.UpdateDescription(request.Description);

        if (request.ImageFile is not null)
        {
            var uploadResult = await roadmapWorkspaceImageService.UploadImageAsync(request.ImageFile, cancellationToken);
            workspace.UpdateImageUrl(uploadResult.RelativePath);
        }

        await repository.UpdateAsync(workspace, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
