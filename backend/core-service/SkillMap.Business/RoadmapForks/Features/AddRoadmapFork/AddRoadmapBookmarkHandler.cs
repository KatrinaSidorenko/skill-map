using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapBookmarks.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapBookmarks.Features.AddRoadmapBookmark;

[UsedImplicitly]
internal sealed class AddRoadmapBookmarkHandler(IRepository<RoadmapWorkspace> repository, IEventBus eventBus) : IRequestHandler<AddRoadmapBookmarkCommand, long>
{
    public async Task<long> Handle(AddRoadmapBookmarkCommand request, CancellationToken cancellationToken)
    {
        var roadmapBookmark = new RoadmapWorkspace(request.UserId, request.RoadmapId);
        await repository.AddAsync(roadmapBookmark, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapBookmarkAddedEvent.Create(roadmapBookmark.Id, roadmapBookmark.RoadmapId, true), cancellationToken);
        return roadmapBookmark.Id;
    }
}
