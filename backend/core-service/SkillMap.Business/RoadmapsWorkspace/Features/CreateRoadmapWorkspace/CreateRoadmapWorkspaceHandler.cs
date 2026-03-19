using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateRoadmapWorkspace;

[UsedImplicitly]
internal sealed class CreateRoadmapWorkspaceHandler(IRepository<RoadmapWorkspace> repository, IEventBus eventBus) : IRequestHandler<CreateRoadmapWorkspaceCommand, long>
{
    public async Task<long> Handle(CreateRoadmapWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var roadmapBookmark = new RoadmapWorkspace(request.UserId, request.RoadmapId, null);
        roadmapBookmark.SetMetadata(new RoadmapWorkspaceMetadata("Who knows the roadmap?", string.Empty, string.Empty));
        // todo: go to external store to get the roadmap data
        await repository.AddAsync(roadmapBookmark, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapWorkspaceCreatedEvent.Create(roadmapBookmark.Id, roadmapBookmark.RoadmapId, false), cancellationToken);
        return roadmapBookmark.Id;
    }
}
