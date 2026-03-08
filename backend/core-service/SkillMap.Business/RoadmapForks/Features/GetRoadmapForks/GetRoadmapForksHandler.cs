
using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapBookmarks.Features.GetRoadmapBookmarks;

[UsedImplicitly]
internal sealed class GetRoadmapForksHandler(IRepository<RoadmapWorkspace> repository) : IRequestHandler<GetRoadmapForksQuery, RoadmapForksDto>
{
    public async Task<RoadmapForksDto> Handle(GetRoadmapForksQuery request, CancellationToken cancellationToken)
    {
        // todo: what to do with autority ??
        var bookmarks = await repository.GetAllAsync(
            x => x.AuthorId == request.UserId && x.IsActive == request.IsActive, 
            ct: cancellationToken);
        return RoadmapForksDto.Create(bookmarks);
    }
}
