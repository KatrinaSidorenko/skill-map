using System.Threading;

using JetBrains.Annotations;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateEmptyRoadmapWorkspace;

[UsedImplicitly]
internal sealed class CreateEmptyRoadmapWorkspaceHandler(
    IRepository<RoadmapWorkspace> repository,
    IMediator mediator,
    IRoadmapWorkspaceImageService roadmapWorkspaceImageService,
    ILogger<CreateEmptyRoadmapWorkspaceHandler> logger) : IRequestHandler<CreateEmptyRoadmapWorkspaceCommand, long>
{
    public async Task<long> Handle(CreateEmptyRoadmapWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var roadmapWorkspace = new RoadmapWorkspace(request.UserId, roadmapId: RoadmapWorkspaceConstants.EmptyWorkspaceKey, personalRoadmapId: null);
        var imageUploadResult = request.ImageFile != null ? await roadmapWorkspaceImageService.UploadImageAsync(request.ImageFile, cancellationToken) : null;
        roadmapWorkspace.SetMetadata(request.Title, request.Description, imageUploadResult?.RelativePath);

        try
        {
            await repository.AddAsync(roadmapWorkspace, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var command = new BuildWorkspaceSnapshotCommand(roadmapWorkspace.Id, roadmapWorkspace.RoadmapId);
            await mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            if (roadmapWorkspace is not null)
            {
                await repository.DeleteAsync(roadmapWorkspace.Id, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);
            }

            logger.LogError(ex, "An error occurred while creating roadmap workspace for user {UserId} and roadmap {RoadmapId}", request.UserId, RoadmapWorkspaceConstants.EmptyWorkspaceKey);
            throw;
        }

        return roadmapWorkspace.Id;
    }
}
