using JetBrains.Annotations;

using MediatR;

namespace SkillMap.Business.RoadmapsWorkspace.Features.UpdateRoadmapWorkspace;

[UsedImplicitly]
internal sealed class UpdateRoadmapWorkspaceHandler(IRoadmapWorkspaceRepository repository, IRoadmapWorkspaceImagesService roadmapWorkspaceImageService) : IRequestHandler<UpdateRoadmapWorkspaceCommand>
{
    public async Task Handle(UpdateRoadmapWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await repository.GetUserActiveWorkspace(request.WorkspaceId, cancellationToken);

        if (request.Title is not null)
            workspace.UpdateTitle(request.Title);

        if (request.Description is not null)
            workspace.UpdateDescription(request.Description);

        var workspacePrevImageUrl = workspace.ImageUrl;
        if (request.ImageFile is not null)
        {
            
            var uploadResult = await roadmapWorkspaceImageService.UploadImageAsync(request.ImageFile, cancellationToken);
            workspace.UpdateImageUrl(uploadResult.RelativePath);
        }

        await repository.UpdateAsync(workspace, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        if (request.ImageFile is null) return; 
        if (string.IsNullOrEmpty(workspacePrevImageUrl)) return;
        // todo: can be extrcated to seperate service
        var deleteImageResult = await roadmapWorkspaceImageService.DeleteImageAsync(workspacePrevImageUrl, cancellationToken);
        if (!deleteImageResult)
        {
            // Log the failure to delete the old image, but continue with the update process
            // as the new image will still be uploaded and set.
        }
    }
}
