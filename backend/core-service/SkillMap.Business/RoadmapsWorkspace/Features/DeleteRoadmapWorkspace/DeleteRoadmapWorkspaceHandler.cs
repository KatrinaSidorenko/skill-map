using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapsWorkspace.Features.DeleteRoadmapFork;

[UsedImplicitly]
internal sealed class DeleteRoadmapWorkspaceHandler(IRepository<RoadmapWorkspace> repository) : IRequestHandler<DeleteWorkspaceCommand>
{
    public async Task Handle(DeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var bookmark = await repository.GetByIdAsync(request.WorkspaceId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), request.WorkspaceId.ToString());

        bookmark.Deactivate();
        await repository.UpdateAsync(bookmark, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
