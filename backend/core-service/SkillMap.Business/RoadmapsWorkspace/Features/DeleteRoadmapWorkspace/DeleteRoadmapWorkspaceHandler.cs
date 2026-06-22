using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace.Features.DeleteRoadmapFork;

[UsedImplicitly]
internal sealed class DeleteRoadmapWorkspaceHandler(IRepository<RoadmapWorkspace> repository, IRoadmapWorkspaceImagesService roadmapWorkspaceImageService) : IRequestHandler<DeleteWorkspaceCommand>
{
    public async Task Handle(DeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await repository.GetFirstOrDefaultAsync(w => w.Id == request.WorkspaceId, cancellationToken) ?? throw new KeyNotFoundException("Roadmap workspace not found.");

        if (request.IsSoftDelete)
        {
            workspace.Deactivate();
            await repository.UpdateAsync(workspace, cancellationToken);
        }
        else
        {
            await repository.DeleteAsync(workspace.Id, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);

        if (!request.IsSoftDelete && !string.IsNullOrEmpty(workspace.ImageUrl))
        {
            var isSuccess = await roadmapWorkspaceImageService.DeleteImageAsync(workspace.ImageUrl, cancellationToken);
            if (!isSuccess)
            {
                // log error + some return or dead letter box
            }
        }

    }
}