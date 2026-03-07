using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapBookmarks;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapBookmarks.Features.DeleteRoadmapBookmark;

[UsedImplicitly]
internal sealed class DeleteRoadmapBookmarkHandler(IRepository<RoadmapBookmark> repository) : IRequestHandler<DeleteRoadmapBookmarkCommand>
{
    public async Task Handle(DeleteRoadmapBookmarkCommand request, CancellationToken cancellationToken)
    {
        var bookmark = await repository.GetByIdAsync(request.BookmarkId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapBookmark), request.BookmarkId.ToString());

        bookmark.Deactivate();
        await repository.UpdateAsync(bookmark, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
