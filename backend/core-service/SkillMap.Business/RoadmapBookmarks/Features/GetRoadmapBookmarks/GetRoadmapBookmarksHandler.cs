
using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapBookmarks;

namespace SkillMap.Business.RoadmapBookmarks.Features.GetRoadmapBookmarks;

[UsedImplicitly]
internal sealed class GetRoadmapBookmarksHandler(IRepository<RoadmapBookmark> repository) : IRequestHandler<GetRoadmapBookmarksQuery, RoadmapBookmarksDto>
{
    public async Task<RoadmapBookmarksDto> Handle(GetRoadmapBookmarksQuery request, CancellationToken cancellationToken)
    {
        var bookmarks = await repository.GetAllAsync(
            x => x.UserId == request.UserId && x.IsActive == request.IsActive, 
            ct: cancellationToken);
        return RoadmapBookmarksDto.Create(bookmarks);
    }
}
