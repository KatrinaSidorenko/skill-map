using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateEmptyRoadmapWorkspace;

[UsedImplicitly]
internal sealed class CreateEmptyRoadmapWorkspaceHandler(
    IRepository<RoadmapWorkspace> repository,
    IEventBus eventBus) : IRequestHandler<CreateEmptyRoadmapWorkspaceCommand, long>
{
    public async Task<long> Handle(CreateEmptyRoadmapWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var roadmapWorkspace = new RoadmapWorkspace(request.UserId, roadmapId: null, personalRoadmapId: null);
        roadmapWorkspace.SetMetadata(request.Title, request.Description, request.ImageUrl);

        await repository.AddAsync(roadmapWorkspace, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapWorkspaceCreatedEvent.Create(roadmapWorkspace.Id, roadmapId: null, isInAuthorMode: false), cancellationToken);

        return roadmapWorkspace.Id;
    }
}
